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
	public partial class ProductManagerPage : ContentPage {
		ProductManagerViewModel viewModel;

		public ProductManagerPage() {
			InitializeComponent();

			BindingContext = viewModel = new ProductManagerViewModel();
		}

		protected override void OnAppearing() {
			base.OnAppearing();
			productListView.ItemsSource = LocalGlobals.Restaurant.Products.OrderBy(p => p.Name);

			viewModel.pickerCats = new List<string>();
			viewModel.pickerCats.Add("All");
			viewModel.pickerCats.AddRange(LocalGlobals.Restaurant
											.ProductCategories
											.Select(c => c.Name)
											.ToList());
			picker.ItemsSource = viewModel.pickerCats;
			picker.SelectedIndex = 0;
		}

		async void HandleItemPressed(object sender, ItemTappedEventArgs e) {
			if (e.Item == null)
				return;

			await Navigation.PushModalAsync(new NavigationPage(new ProductEditPage(((Product)e.Item).ProductId)));

			((ListView)sender).SelectedItem = null;
		}

		private void OnCategoryIndexChanged(object sender, EventArgs e) {
			var picker = (Picker)sender;
			int selectedIndex = picker.SelectedIndex;

			if (selectedIndex == 0) {
				productListView.ItemsSource = LocalGlobals.Restaurant.Products.OrderBy(p => p.Name);
			} else if (selectedIndex != -1) {
				var productIds = LocalGlobals.Restaurant.ProductCategories.FirstOrDefault(c => c.Name == (string)picker.ItemsSource[selectedIndex]).ProductIds;
				productListView.ItemsSource = LocalGlobals.Restaurant
											.Products
											.Where(p => productIds.Contains(p.ProductId))
											.OrderBy(p => p.Name);
			}
		}

		async void AddItemPressed(object sender, EventArgs e) {
			await Navigation.PushModalAsync(new NavigationPage(new ProductEditPage()));
		}
	}
}
