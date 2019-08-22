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
	public partial class ProductEditPage : ContentPage {
		ProductEditViewModel viewModel;
		bool isNew = false;

		public ProductEditPage() {
			InitializeComponent();

			BindingContext = viewModel = new ProductEditViewModel();
			isNew = true;
			deleteButton.IsVisible = false;
		}
		public ProductEditPage(Guid productId) {
			InitializeComponent();

			BindingContext = viewModel = new ProductEditViewModel(productId);
		}

		async void CancelPressed(object sender, EventArgs e) {
			await Navigation.PopModalAsync();
		}

		async void SavePressed(object sender, EventArgs e) {
			// Update the product item
			if (!isNew) {
				var prod = LocalGlobals.Restaurant.Products
					.FirstOrDefault(c => c.ProductId== viewModel.Product.ProductId);
				LocalGlobals.Restaurant.Products.Remove(prod);
			}

			LocalGlobals.Restaurant.Products.Add(viewModel.Product);
			LocalGlobals.Restaurant.Version++;
			LocalGlobals.SaveRestaurant();
			await LocalGlobals.UpdateRestaurant(LocalGlobals.Restaurant);
			await Navigation.PopModalAsync();
		}

		async void DeletePressed(object sender, EventArgs e) {
			if (!isNew) {
				var response = await DisplayAlert(viewModel.Product.Name,
										"Are you sure you want to delete this category?",
										"Delete", "Cancel");

				if (response == true) {
					var prod = LocalGlobals.Restaurant.Products.First(p => p.ProductId == viewModel.Product.ProductId);
					LocalGlobals.Restaurant.Products.Remove(prod);
					LocalGlobals.Restaurant.Version++;

					await LocalGlobals.UpdateRestaurant(LocalGlobals.Restaurant);
					await Navigation.PopModalAsync();
				}
			}
		}
	}
}