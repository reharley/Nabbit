using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.Models {
	public class OrderEntity : TableEntity {
		public static string PartitionKeyLabel = "order";
		public OrderEntity() { }
		public OrderEntity(string orderId) {
			this.PartitionKey = PartitionKeyLabel;
			this.RowKey = orderId;
		}

		public OrderEntity(string orderId, string json) {
			this.PartitionKey = PartitionKeyLabel;
			this.RowKey = orderId;

			JSON = json;
		}

		public string JSON { get; set; }
	}
}
