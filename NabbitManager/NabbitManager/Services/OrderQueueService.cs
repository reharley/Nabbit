using Nabbit.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace NabbitManager.Services {
	public static class OrderQueueService {
#if DEBUG
		private const string getQueueOrdersUrl = "https://nabbitdev.azurewebsites.net/api/GetQueueOrders/restaurantId/{restaurantId}/allOrders/{allOrders}?code=arzTwHbEMUeCaWjjRlBHpykC3D8vjO2Ii9mto9HpURocnB/dgB2lIQ==";
		private const string deleteQueueOrdersUrl = "https://nabbitdev.azurewebsites.net/api/DeleteQueueOrder/restaurantId/{restaurantId}/orderId/{orderId}/orderNumber/{orderNumber}?code=IEy1H8aKmk4qhj1LZma9kPzSJWSUC/72iBOqRoFjZcWkDsm0UM1Veg==";
#else
		private const string getQueueOrdersUrl = "https://nabbit.azurewebsites.net/api/GetQueueOrders/restaurantId/{restaurantId}/allOrders/{allOrders}?code=kXf6aapwz4ScevooekM/4H5INYPzaLsr54IXOFeyG6Xm6SxBR2p9VQ==";
		private const string deleteQueueOrdersUrl = "https://nabbit.azurewebsites.net/api/DeleteQueueOrder/restaurantId/{restaurantId}/orderId/{orderId}/orderNumber/{orderNumber}?code=UG1uLRRpacxZnSsBdoMoREMNvX2nDq2rMX1qVxbXHpW6LfgsxKPMSg==";
#endif

		public static int OrderNumber = 0;

		static List<Order> orderQueue;
		public static List<Order> OrderQueue {
			get {
				if (orderQueue == null)
					orderQueue = new List<Order>();

				return orderQueue;
			}
			set {
				orderQueue = value;
			}
		}

		public static async Task<bool> GetQueueOrders (string restaurantId, bool allOrders = false) {
			if (OrderQueue == null)
				OrderQueue = new List<Order>();

			try {
				using (var client = new HttpClient()) {
					var url = getQueueOrdersUrl.Replace("{restaurantId}", restaurantId).Replace("{allOrders}", allOrders.ToString());
					string result = "";
					using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
						result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
						if (httpResponse.IsSuccessStatusCode) {
							var orderList = JsonConvert.DeserializeObject<List<Order>>(result).ToList();
							if (allOrders)
								OrderQueue = orderList;
							else if (orderList.Count > 0)
								OrderQueue.AddRange(orderList);

							OrderQueue = OrderQueue.OrderBy(x => x.PickupTime).ToList();


						}

						return httpResponse.IsSuccessStatusCode;
					}
				}
			} catch (Exception ex) {
				/// TODO: Log error
				Console.WriteLine("Something is missing...", "The app was unable to load data. Please check the your connections and try again.");
				Console.WriteLine(ex.Message);
			}

			return false;
		}

		public static async Task<bool> DeleteQueueOrder (string restaurantId, Order order) {
			try {
				using (var client = new HttpClient()) {
					var url = deleteQueueOrdersUrl.Replace("{restaurantId}", restaurantId)
						.Replace("{orderId}", order.OrderId.ToString())
						.Replace("{orderNumber}", order.OrderNumber.ToString());
					string result = "";
					using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
						result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
						if (httpResponse.IsSuccessStatusCode)
							OrderQueue.Remove(order);

						return httpResponse.IsSuccessStatusCode;
					}
				}
			} catch (Exception ex) {
				/// TODO: Log error
				Console.WriteLine("Something is missing...", "The app was unable to load data. Please check the your connections and try again.");
				Console.WriteLine(ex.Message);
			}

			return false;
		}
	}
}
