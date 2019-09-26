using System;
using System.Collections.Generic;
using System.Linq;
using Nabbit.Services;
using NabbitManager.ViewModels;
using Xamarin.Forms;

namespace NabbitManager.Views {
	public partial class MenuEditPage : ContentPage {
		public MenuEditViewModel viewModel { get; set; }
		bool isNew;

		public MenuEditPage () {
			InitializeComponent();

			BindingContext = viewModel = new MenuEditViewModel();
			isNew = true;
		}

		public MenuEditPage (Guid menuId) {
			InitializeComponent();

			isNew = false;
			BindingContext = viewModel = new MenuEditViewModel(menuId);
		}

		async void CancelPressed (object sender, EventArgs e) {
			await Navigation.PopModalAsync();
		}

		async void SavePressed (object sender, EventArgs e) {
			// Update the product item
			if (!isNew) {
				var menu = LocalGlobals.Restaurant.Menus
					.FirstOrDefault(c => c.MenuId == viewModel.Menu.MenuId);
				LocalGlobals.Restaurant.Menus.Remove(menu);
			}

			viewModel.SaveModel();
			LocalGlobals.Restaurant.Menus.Add(viewModel.Menu);
			await LocalGlobals.SaveRestaurant();
			await Navigation.PopModalAsync();
		}

		async void DeletePressed (object sender, EventArgs e) {
			if (!isNew) {
				var response = await DisplayAlert(viewModel.Menu.Name,
										"Are you sure you want to delete this category?",
										"Delete", "Cancel");

				if (response == true) {
					var menu = LocalGlobals.Restaurant.Menus.First(p => p.MenuId == viewModel.Menu.MenuId);
					LocalGlobals.Restaurant.Menus.Remove(menu);

					await LocalGlobals.SaveRestaurant();
					await Navigation.PopModalAsync();
				}
			}
		}
	}
}
