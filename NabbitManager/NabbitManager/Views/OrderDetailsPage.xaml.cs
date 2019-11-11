using Nabbit.Models;
using NabbitManager.Services;
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
	public partial class OrderDetailsPage : ContentPage {
		OrderDetailsViewModel viewModel;

		public OrderDetailsPage (Order order) {
			InitializeComponent();

			BindingContext = viewModel = new OrderDetailsViewModel(order);
		}

		void PrintReceipt (object sender, System.EventArgs e) {
			PrinterService.PrinterAsync(viewModel.Order);
		}
	}
}