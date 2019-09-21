using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nabbit.Models;
using Nabbit.ViewModels;
using Xamarin.Forms;

namespace Nabbit.Views {
	public partial class PaymentMethodsPage : ContentPage {
		PaymentMethodsViewModel viewModel;

		public PaymentMethodsPage () {
			InitializeComponent();
			BindingContext = viewModel = new PaymentMethodsViewModel();
			if (Device.RuntimePlatform == Device.Android) {
				var thick = ((Thickness)App.Current.Resources["AndroidPageMargin"]).Left;
				pageStack.Margin = new Thickness(0, thick, 0, 0);
				addCardButton.WidthRequest = 190;
			}

			PullCustomerIds();
		}

		async Task PullCustomerIds () {
			await viewModel.GetPaymentMethods();
			payMethodsList.ItemsSource = viewModel.PaymentMethods;
		}

		async void OnItemSelected (object sender, SelectionChangedEventArgs e) {
			if (e.CurrentSelection.Count == 0)
				return;

			var payMethod = e.CurrentSelection[0] as PaymentMethod;
			if (payMethod == null)
				return;

			await Task.Delay(2000);
			//await App.Current.MainPage.Navigation.PushAsync(new OrderItemEditPage(payMethod.PaymentMethodId));

			var collection = sender as CollectionView;
			collection.SelectedItem = null;
		}
		
		async void AddCardPressed (object sender, SelectionChangedEventArgs e) {
			if (e.CurrentSelection.Count == 0)
				return;

			await App.Current.MainPage.Navigation.PushAsync(new PaymentInfoPage());

			var collection = sender as CollectionView;
			collection.SelectedItem = null;
		}
	}
}
