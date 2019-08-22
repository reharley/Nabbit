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
	public partial class OrderDetailPage : ContentPage {
		OrderDetailViewModel viewModel;

		public OrderDetailPage(string orderId) {
			InitializeComponent();

			BindingContext = viewModel = new OrderDetailViewModel(orderId);
		}

		protected override void OnAppearing() {
			base.OnAppearing();

			orderItems.ItemsSource = viewModel.OrderItems;
			subTotalLabel.Text = string.Format("{0:C}", viewModel.Subtotal);
			totalLabel.Text = string.Format("{0:C}", viewModel.Total);
		}
	}
}