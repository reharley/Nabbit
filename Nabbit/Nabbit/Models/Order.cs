using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Nabbit.Models {
	public partial class Order : BaseEntity {
		public Order(Guid userId, Guid restaurantId) {
			OrderId = Guid.NewGuid();
			UserId = userId;
			RestaurantId = restaurantId;

			CreationDate = DateTime.Now;
			OrderItems = new List<OrderItem>();
		}

		public Guid OrderId { get; set; }
		public Guid RestaurantId { get; set; }
		public Guid UserId { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }

		public DateTime CreationDate { get; set; }
		public DateTime PickupTime { get; set; }
		public string OrderStatus { get; set; }
		public decimal OrderTotal { get; set; }

		public virtual ICollection<OrderItem> OrderItems { get; set; }
	}

	public static class OrderStatus {
		public const string Creating = "Creating";
		public const string Submitting = "Submitting";
		public const string Transit = "InTransit";
		public const string Submitted = "Submitted";
		public const string Queued = "Queued";
		public const string Processed = "Processed";
	}
}
