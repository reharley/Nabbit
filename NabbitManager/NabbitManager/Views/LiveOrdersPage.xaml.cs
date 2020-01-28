using Nabbit.Models;
using Nabbit.Services;
using Nabbit.Views;
using NabbitManager.Services;
using NabbitManager.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NabbitManager.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LiveOrdersPage : ContentPage {
		LiveOrdersViewModel viewModel;
		Task checkTask;
		private CancellationTokenSource ctsStatus;

		public LiveOrdersPage () {
			InitializeComponent();
			BindingContext = viewModel = new LiveOrdersViewModel();
			viewModel.RefreshOrders(OrderQueueService.LiveOrders);

			if (LocalGlobals.Restaurant.IsActive) {
				stopStartButton.Text = "Stop";
				stopStartButton.BackgroundColor = Color.Red;
			} else {
				stopStartButton.Text = "Start";
				stopStartButton.BackgroundColor = Color.Green;
			}
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
				OrderService.StartOrderProcessing();
			}
		}

		public void StartUpdate () {
			StopUpdate();

			ctsStatus = new CancellationTokenSource();
			checkTask = ActionUpdater(ctsStatus.Token);
		}

		public void StopUpdate () {
			if (ctsStatus != null) ctsStatus.Cancel();

			ctsStatus = null;
		}

		async Task ActionUpdater (CancellationToken ct) {
			int msDelay = 500;
			while (!ct.IsCancellationRequested) {
				if (OrderService.IsConnected == false) {
					actionIndicator.IsVisible = true;

					var currentColor = actionIndicator.BackgroundColor;
					actionIndicator.BackgroundColor = currentColor == Color.Black ? Color.Red : Color.Black;
					actionText.TextColor = Color.White;
					actionText.Text = "RECONNECT NEEDED!";
				} else if (OrderQueueService.NewOrderTimer.TotalSeconds > 0) {
					actionIndicator.IsVisible = true;

					var currentColor = actionIndicator.BackgroundColor;
					actionIndicator.BackgroundColor = currentColor == Color.Blue ? Color.Black : Color.Blue;
					actionText.TextColor = Color.White;
					actionText.Text = "NEW ORDER!";

					var ts = new TimeSpan(0, 0, 0, 0, msDelay);
					ts = OrderQueueService.NewOrderTimer.Subtract(ts);
					if (ts.TotalSeconds < 0)
						ts = new TimeSpan(0, 0, 0);

					OrderQueueService.NewOrderTimer = ts;
				} else {
					actionIndicator.IsVisible = false;
				}

				viewModel.RefreshOrders(OrderQueueService.LiveOrders);
				viewModel.OrderCount = OrderQueueService.LiveOrders.Count + OrderQueueService.OrderQueue.Count;

				await Task.Delay(msDelay, ct);
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

		private async void OrderPressed (object sender, ItemTappedEventArgs e) {
			if (e.Item == null)
				return;

			await Navigation.PushAsync(new OrderDetailsPage((Order)e.Item));

			//Deselect Item
			((ListView)sender).SelectedItem = null;
		}

		private async void StartStopClicked (object sender, EventArgs e) {
			if (LocalGlobals.Restaurant.IsActive) {
				LocalGlobals.Restaurant.IsActive = false;
				stopStartButton.Text = "Start";
				stopStartButton.BackgroundColor = Color.Green;
			} else {
				LocalGlobals.Restaurant.IsActive = true;
				stopStartButton.Text = "Stop";
				stopStartButton.BackgroundColor = Color.Red;
			}

			await LocalGlobals.UpdateRestaurant(LocalGlobals.Restaurant);
		}

		private async void FrameChanged (object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName != "BackgroundColor")
				return;

			var item = (Frame)sender;
			uint timeout = 50;
			await item.TranslateTo(-15, 0, timeout);
			await item.TranslateTo(15, 0, timeout);
			await item.TranslateTo(-10, 0, timeout);
			await item.TranslateTo(10, 0, timeout);
			await item.TranslateTo(-5, 0, timeout);
			await item.TranslateTo(5, 0, timeout);
		}
	}
}