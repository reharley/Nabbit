using System;
using System.Threading;
using System.Threading.Tasks;
using Nabbit.Services;

namespace NabbitManager.Services {
	public static class OrderService {
		public static bool IsConnected { get; set; }

		private static CancellationTokenSource ctsProcessor;
		private static Task orderTask;

		static void StartService () {
			IsConnected = false;
			ctsProcessor = new CancellationTokenSource();
			orderTask = ProcessOrders(ctsProcessor.Token);
		}

		static void StopService () {
			IsConnected = false;
			if (ctsProcessor != null) ctsProcessor.Cancel();
			ctsProcessor = null;
			orderTask = null;
		}

		/// <summary>
		/// Starts the order processing service if the device is set to be the printer.
		/// </summary>
		/// <returns>Returns true if ordering service was able to start</returns>
		public static bool StartOrderProcessing () {
			if (App.Current.Properties.ContainsKey("InstallId") == false)
				App.Current.Properties["InstallId"] = Guid.NewGuid().ToString();

			var installIdString = App.Current.Properties["InstallId"] as string;
			var installId = Guid.Parse(installIdString);
			if (LocalGlobals.Restaurant.PrinterId == installId) {
				if (orderTask != null && orderTask.IsCompleted == false)
					return true;

				StartService();
				return true;
			} else {
				return false;
			}
		}

		static async Task ProcessOrders (CancellationToken ct) {
			var lastPing = LocalGlobals.Restaurant.LastPing;
			var restaurantId = LocalGlobals.Restaurant.RestaurantId.ToString();

			await OrderQueueService.GetQueueOrders(restaurantId, true);
			while (!ct.IsCancellationRequested) {
				if (LocalGlobals.Restaurant.IsOpen() == false)
					continue;

				var timeDiff = DateTime.Now.Subtract(lastPing);
				if (timeDiff > new TimeSpan(0, LocalGlobals.PingMinuteDelay, 0)) {
					IsConnected = await OrderQueueService.GetQueueOrders(restaurantId);
					if (IsConnected) {
						lastPing = DateTime.Now;
						LocalGlobals.Restaurant.LastPing = lastPing;
						IsConnected = await LocalGlobals.UpdateRestaurant(LocalGlobals.Restaurant);
					}
				}

				if (OrderQueueService.OrderQueue.Count > 0) {
					var order = OrderQueueService.OrderQueue[0];
					DateTime pickupPrintTime = order.PickupTime.AddMinutes(-10);
					if (order.PickupTime.Minute < 10) {
						pickupPrintTime = order.PickupTime
							.AddHours(-1)
							.AddMinutes(50);
					}

					if (DateTime.Now >= pickupPrintTime) {
						OrderQueueService.OrderNumber++;
						order.OrderNumber = OrderQueueService.OrderNumber;
						await PrinterService.PrinterAsync(order);
						await Task.Delay(3000, ct);
						IsConnected = await OrderQueueService.DeleteQueueOrder(restaurantId, order);
					}
				}

				if (IsConnected)
					await Task.Delay(1000, ct);
				else
					await Task.Delay(200, ct);
			}

			ct.ThrowIfCancellationRequested();
		}

		
	}
}
