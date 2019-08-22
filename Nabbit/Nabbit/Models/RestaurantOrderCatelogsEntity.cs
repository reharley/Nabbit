using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.Models {
	public class RestaurantOrderCatelogsEntity : TableEntity {
		public static string PartitionKeyLabel = "restaurant_order";
		public RestaurantOrderCatelogsEntity() { }
		public RestaurantOrderCatelogsEntity(string restaurantId) {
			PartitionKey = PartitionKeyLabel;
			RowKey = restaurantId;
		}

		public RestaurantOrderCatelogsEntity(string restaurantId, string json) {
			PartitionKey = PartitionKeyLabel;
			RowKey = restaurantId;
			JSON = json;
		}

		public string JSON { get; set; }
	}
}
