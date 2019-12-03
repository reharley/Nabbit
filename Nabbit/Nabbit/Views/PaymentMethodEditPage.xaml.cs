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

			saveButton.Opacity = .5;
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

			var cardIncomplete = false;

			if (cardNumber == null) {
				cardIncomplete = true;
			} else if (cardNumber.Length < 19) {
				cardIncomplete = true;
				if (cardNumber.Contains("-") == false && cardNumber.Length == 16)
					cardIncomplete = false;
			}

			if (cardIncomplete) {
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
			} else if (zip == null || zip.Length == 0) {
				valid = false;
				await DisplayAlert("Incomplete Form", "Zip is incomplete", "Ok");
			}

			return valid;
		}

		async void SavePressed (object sender, EventArgs e) {
			if (newPayMethod == false)
				return;

			if (await VerifyForm() == false)
				return;

			viewModel.SaveModel();
			//viewModel.TestModel();
			var payMethod = viewModel.PayMethod;
			var success = await StripeService.AttachPayMethod(payMethod);
			if (success)
				await Navigation.PopAsync();
			else
				await DisplayAlert("Failed to save", "Card could not be saved. Please check your card details and try again.", "Ok");
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
