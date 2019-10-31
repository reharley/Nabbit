using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Nabbit.Models {
	public class LiveOrdersEntity : TableEntity {
		public static string PartitionKeyLabel = "live_orders";
		public LiveOrdersEntity () { }
		public LiveOrdersEntity (string restaurantId) {
			PartitionKey = PartitionKeyLabel;
			RowKey = restaurantId;
		}

		public LiveOrdersEntity (string restaurantId, string json) {
			PartitionKey = PartitionKeyLabel;
			RowKey = restaurantId;
			JSON = json;
		}

		public string JSON { get; set; }
	}
}
