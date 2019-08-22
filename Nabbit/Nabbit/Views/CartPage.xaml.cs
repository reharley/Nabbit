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
		OrderItemEditPage orderItemPage;

		public CartPage() {
			InitializeComponent();
			BindingContext = viewModel = new CartViewModel();
			viewModel.Cart = Cart.OrderItems;
		}

		protected override void OnAppearing() {
			base.OnAppearing();
			viewModel.Cart = Cart.OrderItems;
			cartCollection.ItemsSource = new List<OrderItem>(viewModel.Cart);
		}

		async void HandleItemPressed(object sender, ItemTappedEventArgs e) {
			if (e.Item == null)
				return;

			await ShowModalPage(((OrderItem)e.Item).OrderItemId);
			((ListView)sender).SelectedItem = null;
			OnAppearing();
		}

		private async Task ShowModalPage(Guid orderItemId) {
			// When you want to show the modal page, just call this method
			// add the event handler for to listen for the modal popping event:
			App.Current.ModalPopping += HandleModalPopping;
			orderItemPage = new OrderItemEditPage(orderItemId, true);
			await App.Current.MainPage.Navigation.PushModalAsync(orderItemPage);
			await Shell.Current.GoToAsync("menu");
		}

		private void HandleModalPopping(object sender, ModalPoppingEventArgs e) {
			if (e.Modal == orderItemPage) {
				// now we can retrieve that phone number:
				orderItemPage = null;
				OnAppearing();
				// remember to remove the event handler:
				App.Current.ModalPopping -= HandleModalPopping;
			}
		}

		private async void CheckoutClicked(object sender, EventArgs e) {
			var user = LocalGlobals.User;
			if (user.LoggedIn == false) {
				await DisplayAlert("Login", "Please create an account before making an order.", "OK");
				await App.Current.MainPage.Navigation.PushModalAsync(new SignInPage());
			} else if (Cart.OrderItems.Count > 0)
					await App.Current.MainPage.Navigation.PushModalAsync(new CheckoutPage());
		}
	}
}