using Nabbit.Models;
using Nabbit.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.ViewModels {
	public class CheckoutViewModel : BaseViewModel {
		public Order Order { get; set; }
		public User User { get; set; }
		public List<PaymentMethod> PaymentMethods { get; set; }

		public DateTime PickupDate { get; set; }
		public TimeSpan PickupTime { get; set; }
		public DateTime MinDate { get; set; }
		public DateTime MaxDate { get; set; }
		public DateTime MinTime { get; set; }
		public DateTime MaxTime { get; set; }

		public string CreditCardNumber { get; set; }
		public string ExpiryDate { get; set; }
		public int CVV { get; set; }

		public CheckoutViewModel() {
			User = LocalGlobals.User;
			Order = new Order(User.UserId, LocalGlobals.Restaurant.RestaurantId);
			Order.FirstName = User.FirstName;
			Order.LastName = User.LastName;
			BuildOrder();
			GetMinMaxProperties();
		}

		void GetMinMaxProperties() {
			MinDate = DateTime.Now;
			MinTime = DateTime.Now;
		}

		void BuildOrder() {
			Order.OrderStatus = OrderStatus.Creating;
			Order.OrderItems = new List<OrderItem>(Cart.OrderItems);
			CalculateOrderCost();
			PickupTime = DateTime.Now.TimeOfDay;
			PickupDate = DateTime.Now;
		}

		void CalculateOrderCost() {
			decimal price = 0m;
			foreach (var orderItem in Order.OrderItems) {
				var itemPrice = orderItem.Product.Price;
				foreach (var addon in orderItem.Addons)
					itemPrice += addon.Price;

				price += itemPrice * orderItem.Quantity;
			}

			Order.OrderTotal = price;
		}
	}
}
