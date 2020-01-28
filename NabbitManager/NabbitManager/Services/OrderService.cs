using System;
using System.Threading;
using System.Threading.Tasks;
using Nabbit.Services;

namespace NabbitManager.Services {
	public static class OrderService {
		public static bool IsConnected { get; set; }
		public static Guid DeviceId { get; set; }

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

			var deviceIdString = App.Current.Properties["InstallId"] as string;
			DeviceId = Guid.Parse(deviceIdString);
			if (LocalGlobals.Restaurant.PrinterId == DeviceId) {
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
				try {
					var timeDiff = DateTime.Now.Subtract(lastPing);
					if (timeDiff > new TimeSpan(0, LocalGlobals.PingMinuteDelay, 0)) {
						var (pingRestuarantResponse, isConnected) = await LocalGlobals
							.PingRestaurant(LocalGlobals.Restaurant.RestaurantId.ToString(),
											DeviceId.ToString());
						lastPing = DateTime.Now;

						if (LocalGlobals.Restaurant.IsOpen() == false) {
							IsConnected = isConnected;
							continue;
						}

						if (pingRestuarantResponse.UpdateRestaurant || !(pingRestuarantResponse.IsDevice)) {
							await LocalGlobals.GetRestaurant();
						}
						if (pingRestuarantResponse.IsDevice == false) {
							StopService();
							break;
						}

						IsConnected = await OrderQueueService.GetQueueOrders(restaurantId);
					}
					
					if (OrderQueueService.OrderQueue.Count > 0) {
						var order = OrderQueueService.OrderQueue[0];
						var delay = LocalGlobals.Restaurant.PickupDelay;
						DateTime pickupPrintTime = order.PickupTime.Subtract(delay);

						if (DateTime.Now >= pickupPrintTime) {
							OrderQueueService.OrderNumber++;
							order.OrderNumber = OrderQueueService.OrderNumber;
							await PrinterService.PrinterAsync(order);
							OrderQueueService.DequeueOrder(order);
							await Task.Delay(3000, ct);
						}
					}

					IsConnected = await OrderQueueService.DeleteQueueOrder();
				} catch (Exception e) {
					var IsHostError = e.GetType().ToString() == "Java.Net.UnknownHostException";
					IsConnected = false;
				}

				if (IsConnected)
					await Task.Delay(1000, ct);
				else
					await Task.Delay(200, ct);

			}

			IsConnected = false;
			ct.ThrowIfCancellationRequested();
		}
	}
}
