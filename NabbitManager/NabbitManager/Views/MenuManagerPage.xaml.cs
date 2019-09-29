using System;
using System.Collections.Generic;
using Nabbit.Services;
using Xamarin.Forms;

namespace NabbitManager.Views {
	public partial class MenuManagerPage : ContentPage {
		public MenuManagerPage () {
			InitializeComponent();
		}

		protected override void OnAppearing () {
			base.OnAppearing();
			menuListView.ItemsSource = LocalGlobals.Restaurant.Menus;
		}

		async void HandleItemPressed (object sender, ItemTappedEventArgs e) {
			if (e.Item == null)
				return;

			await Navigation.PushAsync(new MenuEditPage(((Nabbit.Models.Menu)e.Item).MenuId));

			((ListView)sender).SelectedItem = null;
		}

		async void AddMenuPressed (object sender, EventArgs e) {
			await Navigation.PushAsync(new MenuEditPage());
		}
	}
}
