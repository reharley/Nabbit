using Nabbit.Services;
using Nabbit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nabbit.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CheckoutPage : ContentPage {
		CheckoutViewModel viewModel;

		public CheckoutPage() {
			InitializeComponent();

			BindingContext = viewModel = new CheckoutViewModel();
		}

		private async void PurchaseClicked(object sender, EventArgs e) {
			if (LocalGlobals.User.LoggedIn) {
				var pickupDate = viewModel.PickupDate;
				var pickupTime = viewModel.PickupTime;
				viewModel.Order.PickupTime = new DateTime(pickupDate.Year, pickupDate.Month, pickupDate.Day, pickupTime.Hours, pickupTime.Minutes, pickupTime.Seconds);
				await LocalGlobals.PostOrder(viewModel.Order);
				await App.Current.MainPage.Navigation.PushModalAsync(new ThankYouPage());
			}
		}

		//public string CreateStripeToken(string cardNumber, string cardExpMonth, string cardExpYear, string cardCVC) {
		//	StripeConfiguration.ApiKey = "pk_test_xxxxxxxxxxxxxxxxx";

		//	var tokenOptions = new TokenCreateOptions() {
		//		Card = new TokenCreateOptions() {
					
		//			//Number = cardNumber,
		//			//ExpirationYear = cardExpYear,
		//			//ExpirationMonth = cardExpMonth,
		//			//Cvc = cardCVC
		//		}
		//	};

		//	var tokenService = new TokenService();
		//	Token stripeToken = tokenService.Create(tokenOptions);

		//	return stripeToken.Id; // This is the token
		//}
	}
}