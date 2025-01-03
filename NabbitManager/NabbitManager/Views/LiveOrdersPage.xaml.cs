﻿using Nabbit.Models;
using Nabbit.Services;
using Nabbit.Views;
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
	public partial class LiveOrdersPage : ContentPage {
		LiveOrdersViewModel viewModel;
		private CancellationTokenSource cts;

		public LiveOrdersPage () {
			InitializeComponent();
			BindingContext = viewModel = new LiveOrdersViewModel();
			viewModel.Orders = OrderQueueService.OrderQueue;

			if (LocalGlobals.Restaurant.IsActive) {
				stopStartButton.Text = "Stop";
				stopStartButton.BackgroundColor = Color.Red;
				stateIndicator.BackgroundColor = Color.Green;
			} else {
				stopStartButton.Text = "Start";
				stateIndicator.BackgroundColor = Color.Red;
				stopStartButton.BackgroundColor = Color.Green;
			}
		}

		protected override void OnAppearing () {
			base.OnAppearing();
			StartUpdate();
		}

		protected override void OnDisappearing () {
			base.OnDisappearing();
			StopUpdate();
		}

		private async void OrderPressed (object sender, ItemTappedEventArgs e) {
			if (e.Item == null)
				return;

			await Navigation.PushAsync(new OrderDetailsPage((Order)e.Item));

			//Deselect Item
			((ListView)sender).SelectedItem = null;
		}

		private void RefreshQueue (object sender, EventArgs e) {
			OrderQueueService.GetQueueOrders(LocalGlobals.Restaurant.RestaurantId.ToString()).Wait();
			viewModel.Orders = OrderQueueService.OrderQueue;
		}

		private async void StartStopClicked (object sender, EventArgs e) {
			if (LocalGlobals.Restaurant.IsActive) {
				LocalGlobals.Restaurant.IsActive = false;
				stateIndicator.BackgroundColor = Color.Red;
				stopStartButton.Text = "Start";
				stopStartButton.BackgroundColor = Color.Green;
			} else {
				LocalGlobals.Restaurant.IsActive = true;
				stopStartButton.Text = "Stop";
				stateIndicator.BackgroundColor = Color.Green;
				stopStartButton.BackgroundColor = Color.Red;
			}

			await LocalGlobals.UpdateRestaurant(LocalGlobals.Restaurant);
		}

		public void StartUpdate () {
			if (cts != null) cts.Cancel();
			cts = new CancellationTokenSource();
			var ignore = UpdaterAsync(cts.Token);
		}

		public void StopUpdate () {
			if (cts != null) cts.Cancel();
			cts = null;
		}

		public async Task UpdaterAsync (CancellationToken ct) {
			while (!ct.IsCancellationRequested) {
				viewModel.Orders = OrderQueueService.OrderQueue;
				await Task.Delay(300, ct);
			}
			ct.ThrowIfCancellationRequested();
		}
	}
}