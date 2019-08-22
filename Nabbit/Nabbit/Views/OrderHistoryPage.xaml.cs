using Nabbit.Models;
using Nabbit.Services;
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
		public OrderHistoryPage() {
			InitializeComponent();
		}

		protected override void OnAppearing() {
			base.OnAppearing();
			orderListView.ItemsSource = LocalGlobals.UserOrders;
		}

		async void HandleItemPressed(object sender, ItemTappedEventArgs e) {
			if (e.Item == null)
				return;

			await Navigation.PushModalAsync(new NavigationPage(new OrderDetailsPage(((Order)e.Item))));

			((ListView)sender).SelectedItem = null;
		}
	}
}