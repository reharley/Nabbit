using System;
using System.Collections.Generic;
using Nabbit.Services;
using NabbitManager.ViewModels;
using Xamarin.Forms;

namespace NabbitManager.Views {
	public partial class RestaurantDetailsPage : ContentPage {
		RestaurantDetailsViewModel viewModel;

		public RestaurantDetailsPage () {
			InitializeComponent();

			BindingContext = viewModel = new RestaurantDetailsViewModel();
		}

		private async void SaveClicked (object sender, EventArgs e) {
			viewModel.SaveModel();

			await LocalGlobals.SaveRestaurant();
			await Navigation.PopAsync();
		}
	}
}
