using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nabbit.Models {
	public partial class OrderItem {
		public OrderItem() {
		}

		public Guid OrderItemId { get; set; }
		public string Instructions { get; set; }
		public int Quantity { get; set; }


		public virtual Product Product { get; set; }
		public virtual List<Addon> Addons { get; set; }
	}
}
