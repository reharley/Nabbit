using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Nabbit.Models {
	public class Order : BaseEntity {
		public Order (Guid userId, Guid restaurantId) {
			OrderId = Guid.NewGuid();
			UserId = userId;
			RestaurantId = restaurantId;
			OrderNumber = 0;

			CreationDate = DateTime.Now;
			OrderItems = new List<OrderItem>();
		}

		public Guid OrderId { get; set; }
		public Guid RestaurantId { get; set; }
		public Guid MenuId { get; set; }
		public Guid UserId { get; set; }
		public Guid TransactionId { get; set; }
		public int OrderNumber { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }

		public DateTime CreationDate { get; set; }
		public DateTime PickupTime { get; set; }
		public string OrderStatus { get; set; }
		public decimal OrderTotal { get; set; }
		public decimal ServiceCharge { get; set; }
		public decimal OrderSubtotal { get; set; }
		public decimal OrderTaxes { get; set; }

		public List<OrderItem> OrderItems { get; set; }
	}

	public static class OrderStatus {
		public const string Created = "Created";
		public const string Queued = "Queued";
		public const string Complete = "Complete";
	}
}
