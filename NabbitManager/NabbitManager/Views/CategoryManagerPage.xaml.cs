using Nabbit.Models;
using Nabbit.Services;
using NabbitManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NabbitManager.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CategoryManagerPage : ContentPage {
		CategoryManagerViewModel viewModel;

		public CategoryManagerPage() {
			InitializeComponent();

			BindingContext = viewModel = new CategoryManagerViewModel();
		}

		protected override void OnAppearing() {
			base.OnAppearing();
			categoryListView.ItemsSource = LocalGlobals.Restaurant.ProductCategories.OrderBy(c => c.Rank);
		}

		async void HandleItemPressed(object sender, ItemTappedEventArgs e) {
			if (e.Item == null)
				return;

			await Navigation.PushModalAsync(new NavigationPage(new CategoryEditPage(((ProductCategory)e.Item).ProductCategoryId)));

			((ListView)sender).SelectedItem = null;
		}

		async void AddCatPressed(object sender, EventArgs e) {
			await Navigation.PushModalAsync(new NavigationPage(new CategoryEditPage()));
		}
	}
}
