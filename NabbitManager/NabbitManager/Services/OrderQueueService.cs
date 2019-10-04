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
		private const string getQueueOrdersUrl = "https://nabbit.azurewebsites.net/api/GetQueueOrders/restaurantId/{restaurantId}?code=kXf6aapwz4ScevooekM/4H5INYPzaLsr54IXOFeyG6Xm6SxBR2p9VQ==";
		private const string deleteQueueOrdersUrl = "https://nabbit.azurewebsites.net/api/GetQueueOrders/restaurantId/{restaurantId}?code=kXf6aapwz4ScevooekM/4H5INYPzaLsr54IXOFeyG6Xm6SxBR2p9VQ==";
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

		public static async Task GetQueueOrders(string restaurantId) {
			if (OrderQueue == null)
				OrderQueue = new List<Order>();

			try {
				using (var client = new HttpClient()) {
					var url = getQueueOrdersUrl.Replace("{restaurantId}", restaurantId);
					string result = "";
					using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
						result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
					}
					if (result != null || result != "")
						OrderQueue.AddRange(JsonConvert.DeserializeObject<List<Order>>(result));
					OrderQueue = OrderQueue.OrderBy(x => x.PickupTime).ToList();
				}
			} catch (Exception ex) {
				/// TODO: Log error
				Console.WriteLine("Something is missing...", "The app was unable to load data. Please check the your connections and try again.");
				Console.WriteLine(ex.Message);
			}
		}

		public static async Task DeleteQueueOrder (string restaurantId, string orderId) {
			try {
				using (var client = new HttpClient()) {
					var url = deleteQueueOrdersUrl.Replace("{restaurantId}", restaurantId).Replace("{orderId}", orderId);
					string result = "";
					using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
						result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
					}
				}
			} catch (Exception ex) {
				/// TODO: Log error
				Console.WriteLine("Something is missing...", "The app was unable to load data. Please check the your connections and try again.");
				Console.WriteLine(ex.Message);
			}
		}
	}
}
