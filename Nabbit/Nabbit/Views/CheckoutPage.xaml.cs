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

		async void OnItemSelected (object sender, SelectionChangedEventArgs e) {
			if (e.CurrentSelection.Count == 0)
				return;

			var payMethod = e.CurrentSelection[0] as PaymentMethod;
			if (payMethod == null)
				return;

			await Navigation.PushAsync(new PaymentMethodEditPage(payMethod));

			var collection = sender as CollectionView;
			collection.SelectedItem = null;
		}

		async void AddCardPressed (object sender, SelectionChangedEventArgs e) {
			if (e.CurrentSelection.Count == 0)
				return;

			await App.Current.MainPage.Navigation.PushModalAsync(new PaymentInfoPage());

			var collection = sender as CollectionView;
			collection.SelectedItem = null;
		}

		private async void PurchaseClicked(object sender, EventArgs e) {
			if (LocalGlobals.User.LoggedIn) {
				var pickupDate = viewModel.PickupDate;
				var pickupTime = viewModel.PickupTime;
				viewModel.Order.PickupTime = new DateTime(pickupDate.Year, pickupDate.Month, pickupDate.Day, pickupTime.Hours, pickupTime.Minutes, pickupTime.Seconds);
				await LocalGlobals.PostOrder(viewModel.Order);
				await App.Current.MainPage.Navigation.PushAsync(new ThankYouPage());
			}
		}
	}
}