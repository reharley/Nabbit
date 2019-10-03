using Nabbit.Models;
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


		private void OnDatePickerIndexChanged (object sender, EventArgs e) {
			viewModel.ChangeMenuHours(datePicker.SelectedIndex);
		}

		private async void AddCardPressed (object sender, EventArgs e) {
			await Navigation.PushModalAsync(new PaymentInfoPage());
		}

		private async void PurchaseClicked (object sender, EventArgs e) {
			if (payMethodsList.SelectedItem == null) {
				await DisplayAlert("Pay Method", "Please add a payment method to charge.", "Ok");
				return;
			}

			if (LocalGlobals.User.LoggedIn) {
				var pickupDate = viewModel.PickupDate;
				var pickupTime = viewModel.PickupTime;
				viewModel.Order.PickupTime = new DateTime(pickupDate.Year, pickupDate.Month, pickupDate.Day, pickupTime.Hours, pickupTime.Minutes, pickupTime.Seconds);

				var payMethod = payMethodsList.SelectedItem as PaymentMethod;
				var response = await LocalGlobals.Charge(payMethod, viewModel.Order);
				if (response.Status != 200) {
					if (response.Status == 400) {
						await DisplayAlert("Pay Method Error",
							response.Message + " Please check your card details.", "Ok");
					} else {
						await DisplayAlert("Pay Method Error", response.Message, "Ok");
					}

					return;
				}

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

			if ((string)datePicker.SelectedItem == "Today") {
				if (pickupTime < DateTime.Now.TimeOfDay) {
					await DisplayAlert("Pickup Time", "The pickup time entered was before the current time.", "Ok");
					viewModel.SetEarliestTime();
				} else if (pickupTime < DateTime.Now.TimeOfDay.Add(new TimeSpan(0, 4, 0))) {
					await DisplayAlert("Pickup Time", "Please provide time to process the order.", "Ok");
					viewModel.SetEarliestTime();
				}
			} else if (openHours <= pickupTime && pickupTime <= closingHours) {
				menuHoursText.TextColor = (Color)App.Current.Resources["primaryColor"];
			} else if (openHours > pickupTime) {
				await DisplayAlert("Pickup Time", "The pickup time entered was before opening time.", "Ok");
				//menuHoursText.TextColor = (Color)App.Current.Resources["dangerColor"];
				viewModel.SetEarliestTime();
			} else if (closingHours < pickupTime) {
				await DisplayAlert("Pickup Time", "The pickup time entered was after closing time.", "Ok");
				//menuHoursText.TextColor = (Color)App.Current.Resources["dangerColor"];
				viewModel.SetEarliestTime();
			}
		}
	}
}