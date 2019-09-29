using System;
using System.Collections.Generic;
using Nabbit.Models;
using Nabbit.ViewModels;
using Xamarin.Forms;

namespace Nabbit.Views {
	public partial class DealsPage : ContentPage {
		DealsViewModel viewModel;
		public DealsPage () {
			InitializeComponent();

			BindingContext = viewModel = new DealsViewModel();
		}

		async void OnItemSelected (object sender, SelectionChangedEventArgs e) {
			if (e.CurrentSelection.Count == 0)
				return;

			var product = e.CurrentSelection[0] as Product;
			if (product == null)
				return;

			await Navigation.PushAsync(new OrderItemEditPage(product.ProductId, viewModel.Menu.MenuId));

			var collection = sender as CollectionView;
			collection.SelectedItem = null;
		}
	}
}
