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
	public partial class CategoryEditPage : ContentPage {
		CategoryEditViewModel viewModel;
		bool isNew = false;

		public CategoryEditPage () {
			InitializeComponent();

			BindingContext = viewModel = new CategoryEditViewModel();
			isNew = true;
			deleteButton.IsVisible = false;
		}

		public CategoryEditPage (Guid categoryId) {
			InitializeComponent();

			BindingContext = viewModel = new CategoryEditViewModel(categoryId);
		}

		async void CancelPressed (object sender, EventArgs e) {
			await Navigation.PopModalAsync();
		}

		async void SavePressed (object sender, EventArgs e) {
			// Update the product item
			if (!isNew) {
				var cat = LocalGlobals.Restaurant.ProductCategories
					.FirstOrDefault(c => c.ProductCategoryId == viewModel.ProductCategory.ProductCategoryId);
				LocalGlobals.Restaurant.ProductCategories.Remove(cat);
			}

			viewModel.SaveModel();
			LocalGlobals.Restaurant.ProductCategories.Add(viewModel.ProductCategory);
			await LocalGlobals.SaveRestaurant();
			await Navigation.PopModalAsync();
		}

		async void DeletePressed (object sender, EventArgs e) {
			if (!isNew) {
				var response = await DisplayAlert(viewModel.ProductCategory.Name,
										"Are you sure you want to delete this category?",
										"Delete", "Cancel");

				if (response == true) {
					var cat = LocalGlobals.Restaurant.ProductCategories.First(c => c.ProductCategoryId == viewModel.ProductCategory.ProductCategoryId);
					LocalGlobals.Restaurant.ProductCategories.Remove(cat);

					await LocalGlobals.SaveRestaurant();
					await Navigation.PopModalAsync();
				}
			}
		}
	}
}