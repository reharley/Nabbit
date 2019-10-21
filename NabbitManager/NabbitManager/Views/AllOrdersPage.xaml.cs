using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nabbit.Models;
using Nabbit.Services;
using Nabbit.Views;
using NabbitManager.Services;
using NabbitManager.ViewModels;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace NabbitManager.Views {
	public partial class AllOrdersPage : ContentPage {
		AllOrdersViewModel viewModel;
		public AllOrdersPage () {
			InitializeComponent();

			BindingContext = viewModel = new AllOrdersViewModel();
		}


		protected override void OnAppearing () {
			base.OnAppearing();
			viewModel.IsBusy = true;
			LoadOrder();
		}

		async Task LoadOrder () {
			var orders = await LocalGlobals.GetRestOrders();
			orderList.ItemsSource = orders.OrderByDescending(o => o.PickupTime);
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

		void Handle_Clicked (object sender, System.EventArgs e) {
			var order = JsonConvert.DeserializeObject<Order>(orderJson);
			var filename = "nabbit_logo_transparent_outline.bmp";
			//var img_url = DependencyService.Get<IBaseUrl>().Get() + "/" + filename;

			//MemoryStream inMemoryCopy = new MemoryStream();
			//using (FileStream fs = File.OpenRead(path)) {
			//	fs.CopyTo(inMemoryCopy);
			//}
			//var img_bytes = inMemoryCopy.ToArray();

			//string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), filename);
			//var yes = File.Exists(path);
			//var no = File.Exists(img_url);
			//var img_bytes = File.ReadAllBytes(path);

			PrinterService.Printer(order);
		}

		private const string orderJson = "{\"OrderId\":\"7a7a2266-0c59-414f-9218-611eb57353b8\",\"RestaurantId\":\"681a6d33-beac-4928-8172-793c3e981bd5\",\"UserId\":\"5d2b6da2-3f67-4fd0-a3c8-678cbfb9d4f9\",\"FirstName\":\"Robert\",\"LastName\":\"Harley\",\"CreationDate\":\"2019-08-20T21:52:07.652116-07:00\",\"PickupTime\":\"2019-08-20T22:03:00\",\"OrderStatus\":\"Creating\",\"OrderTotal\":21.75,\"OrderItems\":[{\"OrderItemId\":\"4bc80609-3df0-4c23-9734-a0f0ad1b0a67\",\"Instructions\":\"Any special instructions?\",\"Quantity\":3,\"Product\":{\"ProductId\":\"7446a5ea-ea0c-4473-8c14-c1e75a8be27a\",\"Name\":\"Veggie Burger\",\"Description\":null,\"Price\":6.0,\"AddonGroupIds\":null},\"Addons\":[{\"AddonId\":\"913aca40-34c8-4ea5-8ab9-469565417823\",\"Name\":\"Cheese\",\"Price\":0.25},{\"AddonId\":\"0f0b4620-61ed-4f2f-b51f-f92938c1307d\",\"Name\":\"Bacon\",\"Price\":1.0},{\"AddonId\":\"91675768-12b8-486b-8281-c7901dbc334b\",\"Name\":\"sm\",\"Price\":0.0},{\"AddonId\":\"ba5fbe74-f083-42e3-94bb-d92ac2edc07e\",\"Name\":\"White\",\"Price\":0.0}]}],\"CreatedAt\":\"2019-08-20T21:52:07.652025-07:00\"}";
	}
}
