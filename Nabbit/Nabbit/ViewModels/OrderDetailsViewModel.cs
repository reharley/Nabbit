using System;
using System.Collections.Generic;
using System.Linq;
using Nabbit.Models;

namespace Nabbit.ViewModels {
	public class OrderDetailsViewModel : BaseViewModel {
		public Order Order { get; set; }
		public string UID { get; set; }
		public string CreationDate { get; set; }
		public string PickupTime { get; set; }
		public decimal ServiceCharge { get; set; }

		public List<OrderItemView> orderItems;
		public List<OrderItemView> OrderItems {
			get {
				return orderItems;
			}
			set {
				SetProperty(ref orderItems, value);
			}
		}

		public OrderDetailsViewModel (Order order) {
			Order = order;
			OrderItems = Order.OrderItems.Select(x => new OrderItemView(x)).ToList();

			var uid = order.OrderId.ToString();
			UID = uid.Substring(uid.Length - 6).ToUpper().Insert(3, "-");

			CreationDate = Order.CreationDate.ToString("MM/dd/yy hh:mm tt");
			PickupTime = Order.PickupTime.ToString("MM/dd/yy hh:mm tt");
			ServiceCharge = Order.ServiceCharge;
		}

	}
}
