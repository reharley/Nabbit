using Nabbit.Models;
using Nabbit.Services;
using Nabbit.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nabbit.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MenuPage : ContentPage {
		public HomeViewModel viewModel;
		bool openingPage = false;

		public MenuPage () {
			InitializeComponent();

			BindingContext = viewModel = new HomeViewModel();

			if (LocalGlobals.User.LoggedIn == false) {
				SignIn();
			}

			if (LocalGlobals.Restaurant == null) {
				viewModel.IsBusy = true;

				var task = LocalGlobals.PullObjects(forcePull:true);
				task.ContinueWith((getTask) => {
					viewModel.BuildViewModel();
					UpdatePage();
				});
			}
		}

		void UpdatePage () {
			if (viewModel.Menus == null)
				return;

			AdjustGroupListHeight();

			// without sleeping, menu tab does not appear on ios.
			// why does this work? I dunnno.
			Thread.Sleep(100);
			viewModel.IsBusy = false;
			menuTabs.ItemsSource = viewModel.MenuNames;
			if (Device.RuntimePlatform == Device.Android)
				menuTabs.SelectedItem = viewModel.GetMenuName();
		}

		async Task SignIn () {
			await Navigation.PushModalAsync(new SignInPage());
		}

		protected override void OnAppearing () {
			base.OnAppearing();
			if (LocalGlobals.Restaurant != null)
				UpdatePage();
		}

		void AdjustGroupListHeight () {
			while (groupList.Children.Count == 0) {
				Thread.Sleep(10);
			}

			for (int i = 0; i < groupList.Children.Count; i++) {
				var products = viewModel.pcProducts[i].Products;
				int productCount = products.Count;
				//int longProductNameCount = 0;
				//foreach (var prod in products) {
				//	if (prod.Name.Length > 15)
				//		longProductNameCount++;
				//}

				//int productCount = collectionView.ItemsSource.OfType<Product>().Count();
				var stackLayout = groupList.Children[i] as StackLayout;
				var collectionView = stackLayout.Children[1] as CollectionView;

				int labelHeight = 100;

				collectionView.HeightRequest = productCount * labelHeight;
			}
		}

		async void OnItemSelected (object sender, SelectionChangedEventArgs e) {
			if (e.CurrentSelection.Count == 0)
				return;

			if (openingPage)
				return;

			openingPage = true;
			var product = e.CurrentSelection[0] as Product;
			if (product == null)
				return;

			await Navigation.PushAsync(new OrderItemEditPage(product.ProductId, viewModel.SelectedMenu.MenuId));

			var collection = sender as CollectionView;
			collection.SelectedItem = null;

			Thread.Sleep(30);
			openingPage = false;
		}

		void Handle_Swiped (object sender, SwipedEventArgs e) {
			viewModel.ChangeMenu(e.Direction);
			menuTabs.SelectedItem = viewModel.GetMenuName();
		}

		void MenuChanged (object sender, SelectionChangedEventArgs e) {
			viewModel.ChangeMenu((string)e.CurrentSelection[0]);
			UpdatePage();
		}
	}
}