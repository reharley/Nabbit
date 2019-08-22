using Nabbit.Models;
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
	public partial class AddonManagerPage : ContentPage {
		AddonManagerViewModel viewModel;

		public AddonManagerPage() {
			InitializeComponent();

			viewModel = new AddonManagerViewModel();
		}

		protected override void OnAppearing() {
			base.OnAppearing();
			addonListView.ItemsSource = LocalGlobals.Restaurant.Addons.OrderBy(a => a.Name);

			viewModel.pickerGroups = new List<string>();
			viewModel.pickerGroups.Add("All");
			viewModel.pickerGroups.AddRange(LocalGlobals.Restaurant
											.AddonGroups
											.Select(c => c.Name)
											.ToList());
			picker.ItemsSource = viewModel.pickerGroups;
			picker.SelectedIndex = 0;
		}

		async void HandleItemPressed(object sender, ItemTappedEventArgs e) {
			if (e.Item == null)
				return;

			await Navigation.PushModalAsync(new NavigationPage(new AddonEditPage(((Addon)e.Item).AddonId)));

			((ListView)sender).SelectedItem = null;
		}

		private void OnGroupIndexChanged(object sender, EventArgs e) {
			var picker = (Picker)sender;
			int selectedIndex = picker.SelectedIndex;

			if (selectedIndex == 0) {
				addonListView.ItemsSource = LocalGlobals.Restaurant.Addons.OrderBy(p => p.Name);
			} else if (selectedIndex != -1) {
				var addonIds = LocalGlobals.Restaurant.AddonGroups.FirstOrDefault(ag => ag.Name == (string)picker.ItemsSource[selectedIndex]).AddonIds;
				addonListView.ItemsSource = LocalGlobals.Restaurant
												.Addons
												.Where(a => addonIds.Contains(a.AddonId))
												.OrderBy(p => p.Name);
			}
		}

		async void AddAddonPressed(object sender, EventArgs e) {
			await Navigation.PushModalAsync(new NavigationPage(new AddonEditPage()));
		}
	}
}