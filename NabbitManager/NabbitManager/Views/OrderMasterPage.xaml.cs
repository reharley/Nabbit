using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nabbit.Services;
using NabbitManager.Services;
using NabbitManager.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NabbitManager.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OrderMasterPage : ContentPage {
#if DEBUG
		string mode = "DEBUG";
#else
		string mode = "PRODUCTION";
#endif
		OrderMasterViewModel viewModel;
		Task printingScheduler;
		bool IsConnected = false;
		Task checkTask;
		private CancellationTokenSource ctsStatus;

		public OrderMasterPage () {
			InitializeComponent();
			BindingContext = viewModel = new OrderMasterViewModel();

			List<Button> Buttons = new List<Button>();
			List<string> titles = new List<string>() {
				"All Orders",
				"Live Orders"
			};

			var uris = new List<string>() {
			"allOrders","liveOrders"
			};

			navButtons.Children.Clear();
			for (int i = 0; i < titles.Count; i++) {
				var button = new Button() {
					Text = titles[i]
				};
				var uri = uris[i];
				if (i == 0) {
					button.Pressed += async (sender, args) => {
						await Navigation.PushAsync(new AllOrdersPage());
					};
				} else if (i == 1) {
					button.Pressed += async (sender, args) => {
						await Navigation.PushAsync(new LiveOrdersPage());
					};
				}
				navButtons.Children.Add(button);
			}

			modeLabel.Text = mode;
		}


		protected override void OnAppearing () {
			base.OnAppearing();
			Reconnect();
			StartUpdate();
		}

		protected override void OnDisappearing () {
			base.OnDisappearing();
			StopUpdate();
		}

		void Reconnect () {
			viewModel.IsBusy = true;

			if (LocalGlobals.Restaurant == null) {
				LoadRestaurant();
			} else {
				if (OrderService.StartOrderProcessing() == true)
					viewModel.PrinterStatus = "Yes";
				else
					viewModel.PrinterStatus = "No";
				viewModel.IsBusy = false;
			}
		}

		public void StartUpdate () {
			StopUpdate();

			ctsStatus = new CancellationTokenSource();
			checkTask = CheckConnection(ctsStatus.Token);
		}

		public void StopUpdate () {
			if (ctsStatus != null) ctsStatus.Cancel();

			ctsStatus = null;
		}

		async Task CheckConnection (CancellationToken ct) {
			while (!ct.IsCancellationRequested) {
				if (OrderService.IsConnected) {
					if (viewModel.ConnectionStatus != "ONLINE")
						viewModel.ConnectionStatus = "ONLINE";
					viewModel.IsBusy = false;
				} else {
					if (viewModel.ConnectionStatus != "OFFLINE")
						viewModel.ConnectionStatus = "OFFLINE";
				}

				if (LocalGlobals.Restaurant == null) {
					if (viewModel.OpenStatus != "CLOSED")
						viewModel.OpenStatus = "CLOSED";
				} else if (LocalGlobals.Restaurant.IsOpen()) {
					if (viewModel.OpenStatus != "OPEN")
						viewModel.OpenStatus = "OPEN";
				} else {
					if (viewModel.OpenStatus != "CLOSED")
						viewModel.OpenStatus = "CLOSED";
				}

				if (viewModel.ConnectionStatus == "OFFLINE" &&
					viewModel.OpenStatus == "OPEN") {
					reconnectIndicator.IsVisible = true;

					var currentColor = reconnectIndicator.BackgroundColor;
					reconnectIndicator.BackgroundColor = currentColor == Color.Black ? Color.Red : Color.Black;
				} else {
					reconnectIndicator.IsVisible = false;
				}

				await Task.Delay(500, ct);
			}

			ct.ThrowIfCancellationRequested();
		}

		void LoadRestaurant () {
			viewModel.IsBusy = true;

			var task = LocalGlobals.PullObjects(forcePull: true);
			task.ContinueWith(async (getTask) => {
				if (getTask.Result == -1) {
					await Task.Delay(500);
				}

				Reconnect();
			});
		}
	}
}