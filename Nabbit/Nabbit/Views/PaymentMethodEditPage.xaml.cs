using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nabbit.Services;
using Nabbit.ViewModels;
using Stripe;
using Xamarin.Forms;

namespace Nabbit.Views {
	public partial class PaymentMethodEditPage : ContentPage {
		PaymentMethodEditViewModel viewModel;
		bool newPayMethod = false;

		public PaymentMethodEditPage () {
			InitializeComponent();
			BindingContext = viewModel = new PaymentMethodEditViewModel();
			newPayMethod = true;
		}

		public PaymentMethodEditPage (Nabbit.Models.PaymentMethod payMethod) {
			InitializeComponent();
			BindingContext = viewModel = new PaymentMethodEditViewModel(payMethod);

			cancelButton.Text = "Delete";
			cancelButton.BackgroundColor = (Color)App.Current.Resources["dangerColor"];
		}

		async Task<bool> VerifyForm () {
			var valid = true;

			var cardNumber = viewModel.CardNumber;
			var cardExp = viewModel.CardEpirationDate;
			var cvv = viewModel.CardCvv;

			var name = viewModel.PayMethod.PersonName;
			var address1 = viewModel.PayMethod.Address1;
			var address2 = viewModel.PayMethod.Address2;
			var city = viewModel.PayMethod.City;
			var country = viewModel.PayMethod.Country;
			var state = viewModel.PayMethod.State;
			var zip = viewModel.PayMethod.Zip;


			if (cardNumber == null || cardNumber.Length < 19) {
				valid = false;
				await DisplayAlert("Incomplete Form", "Card Number is incomplete", "Ok");
			} else if (cardExp == null || cardExp.Length < 5) {
				valid = false;
				await DisplayAlert("Incomplete Form", "Card Expiration is incomplete", "Ok");
			} else if (cvv == null || cvv.Length < 3) {
				valid = false;
				await DisplayAlert("Incomplete Form", "Card CVV is incomplete", "Ok");
			} else if (name == null || name.Length == 0) {
				valid = false;
				await DisplayAlert("Incomplete Form", "Name is incomplete", "Ok");
			} else if (address1 == null || address1.Length == 0) {
				valid = false;
				await DisplayAlert("Incomplete Form", "Address 1 is incomplete", "Ok");
			} else if (city == null || city.Length == 0) {
				valid = false;
				await DisplayAlert("Incomplete Form", "City is incomplete", "Ok");
			} else if (country == null || country.Length == 0) {
				valid = false;
				await DisplayAlert("Incomplete Form", "Country is incomplete", "Ok");
			} else if (state == null || state.Length == 0) {
				valid = false;
				await DisplayAlert("Incomplete Form", "State is incomplete", "Ok");
			} else if (zip == null || zip.Length == 0) {
				valid = false;
				await DisplayAlert("Incomplete Form", "Zip is incomplete", "Ok");
			}

			return valid;
		}

		async void SavePressed (object sender, EventArgs e) {
			if (await VerifyForm() == false)
				return;

			viewModel.SaveModel();
			//viewModel.TestModel();
			var payMethod = viewModel.PayMethod;


			StripeConfiguration.ApiKey = "pk_test_zFFDBaQm00tzDkEh04fd3vOS000CPjQesc";
			var setupIntent = await StripeService.GetSetupIntentAsync();

			var billing = new BillingDetailsOptions() {
				Name = payMethod.PersonName,
				Address = new AddressOptions() {
					Line1 = payMethod.Address1,
					Line2 = payMethod.Address2,
					City = payMethod.City,
					State = payMethod.State,
					PostalCode = payMethod.Zip,
					Country = payMethod.Country.ToLower()
				}
			};

			var payOptions = new PaymentMethodCreateOptions {
				Type = "card",
				Card = new PaymentMethodCardCreateOptions {
					Number = payMethod.CardNumber,
					ExpMonth = long.Parse(payMethod.MonthExpire),
					ExpYear = long.Parse(payMethod.YearExpire),
					Cvc = payMethod.CVV
				},
				BillingDetails = billing
			};

			var payService = new PaymentMethodService();
			var paymentMethod = await payService.CreateAsync(payOptions);

			var confirmOptions = new SetupIntentConfirmOptions() {
				ClientSecret = setupIntent.ClientSecret,
				PaymentMethodId = paymentMethod.Id
			};

			var setupService = new SetupIntentService();
			try {
				var completeSetupIntent = await setupService.ConfirmAsync(setupIntent.Id, confirmOptions);
				if (completeSetupIntent.Status != "succeeded")
					return;
			} catch (Exception exc) {
				await DisplayAlert("Failed", exc.Message, "Ok");
				return;
			}

			if (LocalGlobals.User.CustomerId == null || LocalGlobals.User.CustomerId == "")
				await LocalGlobals.GetUser();

			bool success = await StripeService.AttachUserPayment(LocalGlobals.User.CustomerId, paymentMethod.Id);
			if (success)
				await Navigation.PopAsync();
			else
				await DisplayAlert("Failed to save", "Card could not be saved. Please try again.", "Ok");
		}

		async void CancelPressed (object sender, EventArgs e) {
			if (newPayMethod == false)
				await StripeService.DetachUserPayment(viewModel.PayMethod.PaymentMethodId);

			await Navigation.PopAsync();
		}

		void Handle_TextChanged (object sender, Xamarin.Forms.TextChangedEventArgs e) {
			var entry = sender as Entry;

			if (e.NewTextValue.ToUpper() != e.NewTextValue)
				entry.Text = e.NewTextValue.ToUpper();
		}
	}
}
