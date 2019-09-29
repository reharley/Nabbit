using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nabbit.Models {
	public class OrderItem {
		public OrderItem() {
		}

		public Guid OrderItemId { get; set; }
		public string Instructions { get; set; }
		public int Quantity { get; set; }
		public decimal ItemPrice { get; set; }

		public Product Product { get; set; }
		public List<Addon> Addons { get; set; }
	}
}
