using System;
using System.Linq;
using System.Collections.Generic;
using Nabbit.Models;
using Nabbit.ViewModels;
using Xamarin.Forms;

namespace NabbitManager.ViewModels {
	public class LiveOrder {
		public LiveOrder (Order order) {
			OrderId = order.OrderId;
			OrderNumber = order.OrderNumber;
			UpdateLiveOrder(order.PickupTime);
		}

		public void UpdateLiveOrder (DateTime pickupTime) {
			var tMinusCountdown = pickupTime.TimeOfDay.Subtract(DateTime.Now.TimeOfDay);

			var plus = tMinusCountdown.TotalSeconds < 0 ? "+" : "-";
			PickupCountdown = "T" + plus + tMinusCountdown.ToString("' 'hh':'mm':'ss");


			var color = Color.LawnGreen;
			if (tMinusCountdown.TotalMinutes < 0)
				color = Color.Red;
			else if (tMinusCountdown.TotalMinutes < 3)
				color = Color.OrangeRed;
			else if (tMinusCountdown.TotalMinutes < 6)
				color = Color.Yellow;

			// check before reassigning to avoid unnecessary property changes.
			if (CardColor != color)
				CardColor = color;
		}

		public Guid OrderId { get; set; }
		public Color CardColor { get; set; }
		public int OrderNumber { get; set; }
		public string PickupCountdown { get; set; }
	}

	public class LiveOrdersViewModel : BaseViewModel {
		List<LiveOrder> orders = new List<LiveOrder>();
		public List<LiveOrder> Orders {
			get {
				return orders;
			}
			set {
				SetProperty(ref orders, value);
			}
		}

		int orderCount = 0;
		public int OrderCount {
			get {
				return orderCount;
			}
			set {
				SetProperty(ref orderCount, value);
			}
		}
		public LiveOrdersViewModel () {
		}

		public void RefreshOrders (List<Order> orders) {
			var updateNeeded = false;
			if (orders.Count > Orders.Count)
				updateNeeded = true;
			foreach (var order in orders) {
				var tmp = Orders.FirstOrDefault(x => x.OrderId == order.OrderId);
				if (tmp == null) {
					Orders.Add(new LiveOrder(order));
					continue;
				}

				tmp.UpdateLiveOrder(order.PickupTime);
			}

			if (updateNeeded)
				Orders = orders.Select(x => new LiveOrder(x)).ToList();
		}
	}
}
