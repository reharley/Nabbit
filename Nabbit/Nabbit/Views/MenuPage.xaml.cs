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

		public MenuPage() {
			InitializeComponent();
			BindingContext = viewModel = new HomeViewModel();
			AdjustGroupListHeight();

			if (LocalGlobals.User.LoggedIn == false) {
				SignIn();
			}

		}

		async Task SignIn() {
			await Navigation.PushModalAsync(new SignInPage());
		}

		void AdjustGroupListHeight () {
			for (int i = 0; i < groupList.Children.Count; i++) {
				var stackLayout = groupList.Children[i] as StackLayout;
				var collectionView = stackLayout.Children[1] as CollectionView;

				int productCount = collectionView.ItemsSource.OfType<Product>().Count();
				int labelHeight = 63, space = 10;
				if (Device.RuntimePlatform == Device.Android)
					labelHeight = 63;

				collectionView.HeightRequest = (productCount * labelHeight) + (productCount * space);
			}
		}

		async void OnItemSelected (object sender, SelectionChangedEventArgs e) {
			if (e.CurrentSelection.Count == 0)
				return;

			var product = e.CurrentSelection[0] as Product;
			if (product == null)
				return;

			await App.Current.MainPage.Navigation.PushAsync(new OrderItemEditPage(product.ProductId));

			var collection = sender as CollectionView;
			collection.SelectedItem = null;
		}
	}
}