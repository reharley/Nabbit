using Nabbit.Models;
using Nabbit.Services;
using Nabbit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nabbit.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OrderHistoryPage : ContentPage {
		OrderHistoryViewModel viewModel;

		public OrderHistoryPage() {
			InitializeComponent();

			BindingContext = viewModel = new OrderHistoryViewModel();
		}

		protected override void OnAppearing() {
			base.OnAppearing();
			viewModel.IsBusy = true;
			LoadOrder();
		}

		async Task LoadOrder () {
			await LocalGlobals.GetUserOrders();
			orderList.ItemsSource = LocalGlobals.UserOrders.OrderByDescending(o => o.PickupTime);
			viewModel.IsBusy = false;
		}

		async void OnItemSelected (object sender, SelectionChangedEventArgs e) {
			if (e.CurrentSelection.Count == 0)
				return;

			var order = e.CurrentSelection[0] as Order;
			if (order == null)
				return;

			await Navigation.PushAsync(new OrderDetailsPage(order));

			var collection = sender as CollectionView;
			collection.SelectedItem = null;
		}
	}
}