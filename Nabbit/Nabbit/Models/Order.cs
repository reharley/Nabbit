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

			CreationDate = DateTime.Now;
			OrderItems = new List<OrderItem>();
		}

		public Guid OrderId { get; set; }
		public Guid RestaurantId { get; set; }
		public Guid MenuId { get; set; }
		public Guid UserId { get; set; }
		public Guid TransactionId { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }

		public DateTime CreationDate { get; set; }
		public DateTime PickupTime { get; set; }
		public string OrderStatus { get; set; }
		public decimal OrderTotal { get; set; }
		public decimal OrderSubtotal { get; set; }
		public decimal OrderTaxes { get; set; }

		public List<OrderItem> OrderItems { get; set; }


		public string GetFattMeta () {
			var objs = new JArray();

			foreach (var orderItem in OrderItems) {
				var obj = new JObject();
				obj["item"] = orderItem.Product.Name;

				if (orderItem.Addons.Count > 0) {
					obj["details"] = "Addons: \n";
					var orderItemView = new OrderItemView(orderItem);
					obj["details"] += orderItemView.AddonText;
				}

				obj["quantity"] = orderItem.Quantity;
				obj["price"] = orderItem.ItemPrice;
				objs.Add(obj);
			}

			return objs.ToString();
		}
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
