using Nabbit.Models;
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
	public partial class AddonGroupManagerPage : ContentPage {
		AddonGroupManagerViewModel viewModel;
		public AddonGroupManagerPage() {
			InitializeComponent();
			BindingContext = viewModel = new AddonGroupManagerViewModel();
		}

		async void HandleItemPressed(object sender, ItemTappedEventArgs e) {
			if (e.Item == null)
				return;
			await Navigation.PushModalAsync(new NavigationPage(new AddonGroupEditPage(((AddonGroup)e.Item).AddonGroupId)));

			((ListView)sender).SelectedItem = null;
		}

		async void AddGroupPressed(object sender, EventArgs e) {
			await Navigation.PushModalAsync(new NavigationPage(new AddonGroupEditPage()));
		}
	}
}