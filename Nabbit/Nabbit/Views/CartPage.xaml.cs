﻿using Nabbit.Models;
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

		public CartPage() {
			InitializeComponent();
			BindingContext = viewModel = new CartViewModel();
			var thiccness = (Thickness) App.Current.Resources["PageMargin"];
			cartList.Margin = new Thickness(thiccness.HorizontalThickness, 0);
		}

		protected override void OnAppearing() {
			base.OnAppearing();
			viewModel.RefreshCart();
		}

		async void HandleItemPressed(object sender, SelectionChangedEventArgs e) {
			if (e.CurrentSelection.Count == 0)
				return;

			var orderItem = e.CurrentSelection[0] as OrderItemView;
			if (orderItem == null)
				return;

			await Navigation.PushAsync(new OrderItemEditPage(orderItem.OrderItemId, Cart.MenuId));

			var collection = sender as CollectionView;
			collection.SelectedItem = null;
		}


		private async void CheckoutClicked(object sender, EventArgs e) {
			if (LocalGlobals.User.LoggedIn == false) {
				await DisplayAlert("Login", "Please create an account before making an order.", "OK");
				await Navigation.PushModalAsync(new SignInPage());
				return;
			}

			viewModel.IsBusy = true;
			await LocalGlobals.GetRestaurant();
			viewModel.IsBusy = false;

			// get how long since last ping
			var now = DateTime.Now;
			var pingTimeSpan = now.Subtract(LocalGlobals.Restaurant.LastPing);
			var minTime = new TimeSpan(0, LocalGlobals.PingMinuteDelay * 2, 0);
			if (minTime < pingTimeSpan)
				LocalGlobals.Restaurant.IsActive = false;

			var user = LocalGlobals.User;
			if (LocalGlobals.Restaurant.IsActive == false) {
				await DisplayAlert("Service Down",
					"Ordering services are down outside of business hours. " +
					"Please try again another time.",
					"OK");
			} else if (Cart.OrderItems.Count > 0)
				await App.Current.MainPage.Navigation.PushAsync(new CheckoutPage());
		}
	}
}