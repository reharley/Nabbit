using Nabbit.Models;
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
			var thiccness = (Thickness)App.Current.Resources["PageMargin"];
			payMethodsList.Margin = new Thickness(thiccness.HorizontalThickness, 0);
		}


		protected override void OnAppearing () {
			base.OnAppearing();
			viewModel.IsBusy = true;
			PullCustomerIds();
		}

		async Task PullCustomerIds () {
			payMethodsList.ItemsSource = await LocalGlobals.GetPaymentMethods();
			viewModel.IsBusy = false;
		}

		private async void AddCardPressed (object sender, EventArgs e) {
			await Navigation.PushModalAsync(new PaymentInfoPage());
			
		}

		private async void PurchaseClicked(object sender, EventArgs e) {
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
	}
}