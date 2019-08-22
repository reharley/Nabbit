using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.Models {
	public class RestaurantOrderCatelogEntity : TableEntity {
		public static string PartitionKeyLabel = "order_catelog";
		public RestaurantOrderCatelogEntity() { }
		public RestaurantOrderCatelogEntity(string catelogId) {
			PartitionKey = PartitionKeyLabel;
			RowKey = catelogId;
		}

		public RestaurantOrderCatelogEntity(string catelogId, string json) {
			PartitionKey = PartitionKeyLabel;
			RowKey = catelogId;
			JSON = json;
		}

		public string JSON { get; set; }
	}
}
