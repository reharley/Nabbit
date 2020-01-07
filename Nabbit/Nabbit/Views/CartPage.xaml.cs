using Microsoft.Identity.Client;
using Nabbit.Models;
using Nabbit.Services;
using Nabbit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nabbit.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CartPage : ContentPage {
		CartViewModel viewModel;

		public CartPage () {
			InitializeComponent();
			BindingContext = viewModel = new CartViewModel();
			var thiccness = (Thickness)App.Current.Resources["PageMargin"];
			cartList.Margin = new Thickness(thiccness.HorizontalThickness, 0);
		}

		protected override void OnAppearing () {
			base.OnAppearing();
			viewModel.RefreshCart();

			if (Cart.OrderItems.Count < 1)
				checkoutButton.IsEnabled = false;
			else
				checkoutButton.IsEnabled = true;
		}

		async void HandleItemPressed (object sender, SelectionChangedEventArgs e) {
			if (e.CurrentSelection.Count == 0)
				return;

			var orderItem = e.CurrentSelection[0] as OrderItemView;
			if (orderItem == null)
				return;

			await Navigation.PushAsync(new OrderItemEditPage(orderItem.OrderItemId, Cart.MenuId));

			var collection = sender as CollectionView;
			collection.SelectedItem = null;
		}


		private async void CheckoutClicked (object sender, EventArgs e) {
			if (LocalGlobals.User.LoggedIn == false) {
				try {
					// Look for existing account
					IEnumerable<IAccount> accounts = await App.AuthenticationClient.GetAccountsAsync();

					AuthenticationResult result = await App.AuthenticationClient
						.AcquireTokenSilent(ADConstants.Scopes, accounts.FirstOrDefault())
						.ExecuteAsync();

					//await Navigation.PushAsync(new LogoutPage(result));
				} catch {
					await DisplayAlert("Login", "Please login or create an account before making an order.", "OK");
					await Navigation.PushModalAsync(new SignInPage());
					return;
				}
			}

			viewModel.IsBusy = true;
			await LocalGlobals.GetRestaurant();
			viewModel.IsBusy = false;


			if (Cart.OrderItems.Count > 0) {
				// get how long since last ping
				var now = DateTime.Now;
				var dayOfWeek = (int)now.DayOfWeek;
				var businessHours = LocalGlobals.Restaurant.BusinessHours;

				if (businessHours.Opening[dayOfWeek] == null) {
					await DisplayAlert("Restaurant Closed",
						"We are not open today. " +
						"Please try again during regular business hours.",
						"OK");
					return;
				} else if (now.TimeOfDay < businessHours.Opening[dayOfWeek]) {
					var openingTime = new DateTime(businessHours.Opening[dayOfWeek].Value.Ticks);
					await DisplayAlert("Restaurant Closed",
									   "We will be open at " +
									   $"{openingTime.ToString("hh:mm tt")}. " +
									   "Please try again during regular business hours.",
									   "OK");
					return;
				} else if (businessHours.Closing[dayOfWeek] < now.TimeOfDay) {
					await DisplayAlert("Restaurant Closed",
						"We are closed for the day. " +
						"Please try again during regular business hours.",
						"OK");
					return;
				}

				var menu = LocalGlobals.Restaurant.Menus.First(m => m.MenuId == Cart.MenuId);
				var menuHours = menu.Hours;

				if (menuHours.Opening[dayOfWeek] == null) {
					await DisplayAlert("Menu Not Available",
						"This menu is not offered today. " +
						"Please order from another menu.",
						"OK");

					Cart.ClearCart();
					return;
				} else if (now.TimeOfDay < menuHours.Opening[dayOfWeek]) {
					//var menuOpeningTime = new DateTime(menuHours.Opening[dayOfWeek].Value.Ticks);
					//await DisplayAlert("Menu Not Available",
					//				   "This menu is not offered yet. " +
					//				   "It will be available at " +
					//				   $"{menuOpeningTime.ToString("hh:mm tt")}.",
					//				   "OK");
					//return;
				} else if (menuHours.Closing[dayOfWeek] < now.TimeOfDay) {
					await DisplayAlert("Menu Not Available",
						"This menu is no longer available today. " +
						"Please try again during regular business hours.",
						"OK");
					return;
				}


				var pingTimeSpan = now.Subtract(LocalGlobals.Restaurant.LastPing);
				var minTime = new TimeSpan(0, LocalGlobals.PingMinuteDelay * 2, 0);
				if (minTime < pingTimeSpan)
					LocalGlobals.Restaurant.IsActive = false;

				if (LocalGlobals.Restaurant.IsActive == false) {
					await DisplayAlert("Service Down",
						"Ordering services are down. " +
						"We are working on the issue.",
						"OK");
					return;
				}

				await App.Current.MainPage.Navigation.PushAsync(new CheckoutPage());
			}
		}
	}
}