﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nabbit.Models;
using Nabbit.Services;
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
	}
}