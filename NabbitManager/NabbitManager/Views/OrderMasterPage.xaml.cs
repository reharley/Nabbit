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
		static OrderMasterViewModel viewModel;
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
			if (LocalGlobals.Restaurant == null) {
				LoadRestaurant();
			} else {
				if (OrderService.StartOrderProcessing() == true)
					viewModel.PrinterStatus = "Yes";
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
				if (OrderService.IsConnected)
					viewModel.ConnectionStatus = "ONLINE";
				else
					viewModel.ConnectionStatus = "OFFLINE";
				await Task.Delay(500, ct);
			}

			ct.ThrowIfCancellationRequested();
		}

		private void ReconnectClicked (object sender, EventArgs e) {
			Reconnect();
		}



		void LoadRestaurant () {
			viewModel.IsBusy = true;
			reconnectButton.IsEnabled = false;

			var task = LocalGlobals.PullObjects(forcePull: true);
			task.ContinueWith((getTask) => {
				if (getTask.Result == -1) {
					viewModel.IsBusy = false;
					reconnectButton.IsEnabled = true;
					return;
				}

				viewModel.IsBusy = false;
				reconnectButton.IsEnabled = false;
				Reconnect();
			});
		}
	}
}