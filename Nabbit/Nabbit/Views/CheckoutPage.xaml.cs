﻿using Nabbit.Models;
using Nabbit.Services;
using Nabbit.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nabbit.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CheckoutPage : ContentPage {
		CheckoutViewModel viewModel;

		public CheckoutPage () {
			InitializeComponent();

			BindingContext = viewModel = new CheckoutViewModel();
			var thiccness = (Thickness)App.Current.Resources["PageMargin"];
			payMethodsList.Margin = new Thickness(thiccness.HorizontalThickness, 0);
			datePicker.SelectedIndex = 0;
		}


		protected override void OnAppearing () {
			base.OnAppearing();
			viewModel.IsBusy = true;
			PullCustomerIds();
		}

		async Task PullCustomerIds () {
			var itemSource = await LocalGlobals.GetPaymentMethods();
			payMethodsList.ItemsSource = itemSource;
			payMethodsList.SelectedItem = itemSource[0];
			viewModel.IsBusy = false;
		}

		void VerifyForm () {
			


			// add holidays
			//if ()
		}

		bool PickupInvalid (DateTime pickupTime) {
			
			return false;
		}

		private void OnDatePickerIndexChanged (object sender, EventArgs e) {
			viewModel.ChangeMenuHours(datePicker.SelectedIndex);
		}

		private async void AddCardPressed (object sender, EventArgs e) {
			await Navigation.PushModalAsync(new PaymentInfoPage());
		}

		private async void PurchaseClicked (object sender, EventArgs e) {
			if (payMethodsList.SelectedItem == null) {
				await DisplayAlert("Pay Method", "Please add a payment method to charge.", "Ok");
			}

			if (LocalGlobals.User.LoggedIn) {
				var pickupDate = viewModel.PickupDate;
				var pickupTime = viewModel.PickupTime;
				viewModel.Order.PickupTime = new DateTime(pickupDate.Year, pickupDate.Month, pickupDate.Day, pickupTime.Hours, pickupTime.Minutes, pickupTime.Seconds);

				var payMethod = payMethodsList.SelectedItem as PaymentMethod;
				await LocalGlobals.Charge(payMethod, viewModel.Order);

				await LocalGlobals.PostOrder(viewModel.Order);
				await Navigation.PushAsync(new ThankYouPage());
			}
		}

		async void OnTimePickerChanged (object sender, PropertyChangedEventArgs e) {
			if (viewModel == null)
				return;

			if (datePicker.SelectedIndex == -1)
				return;

			var pickupDate = viewModel.PickupDateTimes[datePicker.SelectedIndex];
			var pickupTime = viewModel.PickupTime;

			var dayOfWeek = (int)pickupDate.DayOfWeek;
			var hours = viewModel.Menu.Hours;
			var openHours = hours.Opening[dayOfWeek].Value;
			var closingHours = hours.Closing[dayOfWeek].Value;

			if (openHours <= pickupTime && pickupTime <= closingHours) {
				menuHoursText.TextColor = (Color)App.Current.Resources["primaryColor"];
			} else if (openHours > pickupTime) {
				await DisplayAlert("Pickup Time", "The pickup time entered was before opening time.", "Ok");
				//menuHoursText.TextColor = (Color)App.Current.Resources["dangerColor"];
				viewModel.SetEarliestTime();
			} else if (closingHours < pickupTime) {
				await DisplayAlert("Pickup Time", "The pickup time entered was after opening time.", "Ok");
				//menuHoursText.TextColor = (Color)App.Current.Resources["dangerColor"];
				viewModel.SetEarliestTime();
			}
		}
	}
}