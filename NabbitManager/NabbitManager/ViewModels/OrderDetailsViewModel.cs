﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nabbit.Models;
using Nabbit.ViewModels;

namespace NabbitManager.ViewModels {
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
			// is first name in there?
			OrderItems = Order.OrderItems.Select(x => new OrderItemView(x)).ToList();

			var uid = order.UserId.ToString();
			UID = uid.Substring(uid.Length - 7).ToUpper().Insert(3, "-");

			CreationDate = Order.CreationDate.ToString("MM/dd/yy hh:mm tt");
			PickupTime = Order.PickupTime.ToString("MM/dd/yy hh:mm tt");
			ServiceCharge = Order.ServiceCharge;
		}

	}
}
