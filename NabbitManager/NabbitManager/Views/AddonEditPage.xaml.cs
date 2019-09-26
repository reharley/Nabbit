using Nabbit.Models;
using Nabbit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NabbitManager.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddonEditPage : ContentPage {
		bool isNew = false;
		Addon addon;

		public AddonEditPage () {
			InitializeComponent();
			isNew = true;
			deleteButton.IsVisible = false;

			BindingContext = addon = new Addon() {
				AddonId = Guid.NewGuid()
			};
		}

		public AddonEditPage (Guid addonId) {
			InitializeComponent();

			BindingContext = addon = LocalGlobals.Restaurant.Addons.First(a => a.AddonId == addonId);
		}

		async void CancelPressed (object sender, EventArgs e) {
			await Navigation.PopModalAsync();
		}

		async void SavePressed (object sender, EventArgs e) {
			// Update the product item
			if (!isNew) {
				var add = LocalGlobals.Restaurant.Addons
					.FirstOrDefault(a => a.AddonId == addon.AddonId);
				LocalGlobals.Restaurant.Addons.Remove(add);
			}

			LocalGlobals.Restaurant.Addons.Add(addon);
			await LocalGlobals.SaveRestaurant();
			await Navigation.PopModalAsync();
		}

		async void DeletePressed (object sender, EventArgs e) {
			if (!isNew) {
				var response = await DisplayAlert(addon.Name,
										"Are you sure you want to delete this addon?",
										"Delete", "Cancel");

				if (response == true) {
					var add = LocalGlobals.Restaurant.Addons.First(a => a.AddonId == addon.AddonId);
					LocalGlobals.Restaurant.Addons.Remove(add);

					await LocalGlobals.SaveRestaurant();
					await Navigation.PopModalAsync();
				}
			}
		}
	}
}