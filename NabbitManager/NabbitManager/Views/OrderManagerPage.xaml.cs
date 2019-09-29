using ESCPOS_NET.Emitters;
using NabbitManager.Services;
using NabbitManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NabbitManager.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OrderManagerPage : ContentPage {
		OrderManagerViewModel viewModel;
		public OrderManagerPage() {
			InitializeComponent();
			viewModel = new OrderManagerViewModel();
		}

		protected override void OnAppearing() {
			base.OnAppearing();

			Console.WriteLine();
			//orderList.ItemsSource = viewModel.OrderRepo.Get().OrderBy(o => o.CreatedAt);
		}
		async Task PrinterServices() {
			
		}
		

		async void OrderPressed(object sender, ItemTappedEventArgs e) {
			//if (e.Item == null)
			//	return;

			//await Navigation.PushAsync(new OrderDetailPage(((Order)e.Item).Id)));

			////Deselect Item
			//((ListView)sender).SelectedItem = null;
		}
	}
}
