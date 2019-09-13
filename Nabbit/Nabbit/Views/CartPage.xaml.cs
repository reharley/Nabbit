using Nabbit.Models;
using Nabbit.Services;
using Nabbit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nabbit.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CartPage : ContentPage {
		CartViewModel viewModel;
		OrderItemEditPage orderItemPage;

		public CartPage() {
			InitializeComponent();
			BindingContext = viewModel = new CartViewModel();
			var thiccness = (Thickness) App.Current.Resources["PageMargin"];
			cartList.Margin = new Thickness(thiccness.HorizontalThickness, 0);
		}

		protected override void OnAppearing() {
			base.OnAppearing();
			viewModel.RefreshCart();
		}

		async void HandleItemPressed(object sender, SelectionChangedEventArgs e) {
			if (e.CurrentSelection.Count == 0)
				return;

			var orderItem = e.CurrentSelection[0] as OrderItemView;
			if (orderItem == null)
				return;

			await App.Current.MainPage.Navigation.PushAsync(new OrderItemEditPage(orderItem.OrderItemId));

			var collection = sender as CollectionView;
			collection.SelectedItem = null;
		}


		private async void CheckoutClicked(object sender, EventArgs e) {
			var user = LocalGlobals.User;
			if (user.LoggedIn == false) {
				await DisplayAlert("Login", "Please create an account before making an order.", "OK");
				await App.Current.MainPage.Navigation.PushModalAsync(new SignInPage());
			} else if (Cart.OrderItems.Count > 0)
				await App.Current.MainPage.Navigation.PushModalAsync(new CheckoutPage());
		}
	}
}