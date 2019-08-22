using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.Models {
	public class RestaurantEntity : TableEntity {
		public static string PartitionKeyLabel = "restaurant";
		public RestaurantEntity() { }
		public RestaurantEntity(string restaurantId) {
			this.PartitionKey = PartitionKeyLabel;
			this.RowKey = restaurantId;
		}

		public RestaurantEntity(string restaurantId, string json) {
			this.PartitionKey = PartitionKeyLabel;
			this.RowKey = restaurantId;
			JSON = json;
		}

		public string JSON { get; set; }
	}
}
