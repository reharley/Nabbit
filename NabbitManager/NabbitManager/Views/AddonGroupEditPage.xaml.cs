using Nabbit.Services;
using NabbitManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NabbitManager.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddonGroupEditPage : ContentPage {
		AddonGroupEditViewModel viewModel;
		bool isNew = false;

		public AddonGroupEditPage () {
			InitializeComponent();
			BindingContext = viewModel = new AddonGroupEditViewModel();

			isNew = true;
			deleteButton.IsVisible = false;
		}

		public AddonGroupEditPage (Guid addonGroupId) {
			InitializeComponent();
			BindingContext = viewModel = new AddonGroupEditViewModel(addonGroupId);
		}

		async void CancelPressed (object sender, EventArgs e) {
			await Navigation.PopAsync();
		}

		async void SavePressed (object sender, EventArgs e) {
			if (!isNew) {
				var group = LocalGlobals.Restaurant.AddonGroups
					.FirstOrDefault(c => c.AddonGroupId == viewModel.AddonGroup.AddonGroupId);
				LocalGlobals.Restaurant.AddonGroups.Remove(group);
			}

			viewModel.AddonGroup.SelectType = (string)groupPicker.SelectedItem;

			viewModel.AddonGroup.AddonIds = new List<Guid>();
			foreach (var addon in viewModel.Addons) {
				if (addon.Selected)
					viewModel.AddonGroup.AddonIds.Add(addon.Item.AddonId);
			}

			LocalGlobals.Restaurant.AddonGroups.Add(viewModel.AddonGroup);

			await LocalGlobals.SaveRestaurant();
			await Navigation.PopAsync();
		}

		async void DeletePressed (object sender, EventArgs e) {
			if (!isNew) {
				var response = await DisplayAlert(viewModel.AddonGroup.Name,
										"Are you sure you want to delete this group?",
										"Delete", "Cancel");

				if (response == true) {
					var cat = LocalGlobals.Restaurant.AddonGroups.First(c => c.AddonGroupId == viewModel.AddonGroup.AddonGroupId);
					LocalGlobals.Restaurant.AddonGroups.Remove(cat);

					await LocalGlobals.SaveRestaurant();
					await Navigation.PopAsync();
				}
			}
		}
	}
}