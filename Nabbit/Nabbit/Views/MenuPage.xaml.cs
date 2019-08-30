using Nabbit.Models;
using Nabbit.Services;
using Nabbit.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nabbit.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MenuPage : ContentPage {
		public HomeViewModel viewModel;
		bool gridDefined = false;

		public MenuPage() {
			InitializeComponent();
			//BackButtonBehavior backButtonBehavior = new BackButtonBehavior() {
			//	IsEnabled = false
			//};
			//Shell.SetBackButtonBehavior(this, backButtonBehavior);
			//NavigationPage.SetHasBackButton(this, false);
			BindingContext = viewModel = new HomeViewModel();
		}

        /*
		private void ConstructMenu() {
			grid.Children.Clear();
			grid.RowDefinitions = new RowDefinitionCollection();

			if (gridDefined == false) {
				// Product/category name and info
				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(15, GridUnitType.Star) });
				// product/category price and count
				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) });
				// Button to add the item to cart
				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) });
				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

				gridDefined = true;
			}


			int row = 0;
			for (int i = 0; i < viewModel.pcProducts.Count; i++) {
				grid.RowDefinitions.Add(new RowDefinition());
				var catProds = viewModel.pcProducts[i];

				var catNameLabel = new Label() {
					Text = catProds.CategoryName,
					FontSize = 22,
					//TextColor = Color.White,
					BackgroundColor = Color.FromHex("#B00020")
				};

				var catCountLabel = new Label() {
					Text = catProds.Products.Count.ToString(),
					BackgroundColor = Color.LightGray
				};

				grid.Children.Add(catNameLabel, 1, row);
				grid.Children.Add(catCountLabel, 2, row);
				row++;

				foreach (var prod in catProds.Products) {
					grid.RowDefinitions.Add(new RowDefinition());

					var prodNameLabel = new Label() {
						Text = prod.Name,
						FontSize = 16
						//TextColor = Color.White
						//BackgroundColor = Color.LightGoldenrodYellow
					};

					var prodPriceLabel = new Label() {
						Text = String.Format("{0:C}", prod.Price),
						FontAttributes = FontAttributes.Bold
						//TextColor = Color.White
						//BackgroundColor = Color.LightSeaGreen
					};

					var prodButton = new ImageButton() {
						Source = "round_add_button.png",
						WidthRequest = 15,
						HeightRequest = 40,
						BackgroundColor = Color.FromHex("#6200EE"),
						CornerRadius = 5
					};

					prodButton.Clicked += async (sender, args) => {
						string name = string.Format("pd?name=Hello", prod.Name);
						await Navigation.PushModalAsync(new NavigationPage(new OrderItemEditPage(prod.ProductId)));
						//await Shell.Current.GoToAsync($"orderitem?productId={prod.ProductId.ToString()}");
					};

					grid.Children.Add(prodNameLabel, 1, row);
					grid.Children.Add(prodPriceLabel, 2, row);
					grid.Children.Add(prodButton, 3, row);
					row++;
				}
			}
		}
        */

	}
}