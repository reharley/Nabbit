using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Threading;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using Com.OneSignal;
using Microsoft.AppCenter.Auth;
using Com.OneSignal.Abstractions;
using NabbitManager.Services;
using Nabbit.Services;
using System.Threading.Tasks;
using Plugin.SecureStorage;

namespace NabbitManager {
	public partial class App : Application {
		private static Task printingScheduler;
		public App () {
			InitializeComponent();

			MainPage = new AppShell();
		}

		protected override void OnStart () {
			// Handle when your app starts
			var task = LocalGlobals.PullObjects();

			while (!task.IsCompleted)
				Thread.Sleep(3);

			//OneSignal.Current.StartInit("fe020159-a228-44d2-96af-8dc46096a219")
			//				.InFocusDisplaying(OSInFocusDisplayOption.None)
			//				.HandleNotificationReceived(HandleNotificationRecieved)
			//				.EndInit();

			//OneSignal.Current.SetExternalUserId(LocalGlobals.Restaurant.RestaurantId.ToString());
			//"uwp={Your UWP App secret here};" +
			//"ios={Your iOS App secret here}"
			AppCenter.Start("android=21aafc72-dffd-4669-bd97-0b8e7e2b2a63;",
				  typeof(Analytics), typeof(Crashes), typeof(Auth));

			//printingScheduler = ProcessOrders();


			if (App.Current.Properties.ContainsKey("InstallId") == false)
				App.Current.Properties["InstallId"] = Guid.NewGuid().ToString();


			var installIdString = App.Current.Properties["InstallId"] as string;
			var installId = Guid.Parse(installIdString);
			if (LocalGlobals.Restaurant.PrinterId == installId)
				printingScheduler = ProcessOrders();
		}

		private async void HandleNotificationRecieved (OSNotification notification) {
			await OrderQueueService.GetQueueOrders(LocalGlobals.Restaurant.RestaurantId.ToString());
			Console.WriteLine();
		}

		protected override void OnSleep () {
			// Handle when your app sleeps
		}

		protected override void OnResume () {
			// Handle when your app resumes
			OrderQueueService.GetQueueOrders(LocalGlobals.Restaurant.RestaurantId.ToString()).Wait();
		}


		async Task ProcessOrders () {
			var lastPing = LocalGlobals.Restaurant.LastPing;
			var restaurantId = LocalGlobals.Restaurant.RestaurantId.ToString();
			while (true) {
				var timeDiff = DateTime.Now.Subtract(lastPing);
				if (timeDiff > new TimeSpan(0, LocalGlobals.PingMinuteDelay, 0)) {
					await OrderQueueService.GetQueueOrders(restaurantId);
					lastPing = DateTime.Now;
					LocalGlobals.Restaurant.LastPing = lastPing;
					await LocalGlobals.UpdateRestaurant(LocalGlobals.Restaurant);
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
						PrinterService.Printer(order);
						await OrderQueueService.DeleteQueueOrder(restaurantId, order.OrderId.ToString());
						OrderQueueService.OrderQueue.RemoveAt(0);
					}
				}

				await Task.Delay(1000);
			}
		}
	}
}
