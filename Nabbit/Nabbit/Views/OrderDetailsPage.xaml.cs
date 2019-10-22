using Nabbit.Models;
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
	public partial class OrderDetailsPage : ContentPage {
		OrderDetailsViewModel viewModel;
		bool newOrder = false;

		public OrderDetailsPage (Order order, bool newOrder = false) {
			InitializeComponent();

			BindingContext = viewModel = new OrderDetailsViewModel(order);
			this.newOrder = newOrder;
			if (newOrder)
				ShowDialog();
		}

		async void ShowDialog () {
			await DisplayAlert("Thank You!", "Your order has been sent! " +
						"Please show this receipt to the cashier at pickup. " +
						"This recepit can be found in your account page as well.", "Ok");
		}

		protected async override void OnDisappearing () {
			if (newOrder)
				await Navigation.PopToRootAsync();
			base.OnDisappearing();
		}
	}
}