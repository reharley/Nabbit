using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.Models {
	public class UserOrdersEntity : TableEntity {
		public static string PartitionKeyLabel = "user_orders";
		public UserOrdersEntity() { }
		public UserOrdersEntity(string userId) {
			this.PartitionKey = PartitionKeyLabel;
			this.RowKey = userId;
		}

		public UserOrdersEntity(string userId, string json) {
			this.PartitionKey = PartitionKeyLabel;
			this.RowKey = userId;
			JSON = json;
		}

		public string JSON { get; set; }
	}
}
