using Nabbit.Models;
using Nabbit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nabbit.ViewModels {
	public class CheckoutViewModel : BaseViewModel {
		public Order Order { get; set; }
		public Menu Menu { get; set; }
		public Restaurant Restaurant { get; set; }

		public PaymentMethod SelectedPayMethod { get; set; }
		public List<PaymentMethod> PaymentMethods { get; set; }

		List<string> pickupDates;
		public List<string> PickupDates {
			get {
				if (pickupDates == null)
					pickupDates = new List<string>();
				return pickupDates;
			}
			set {
				SetProperty(ref pickupDates, value);
			}
		}
		public List<DateTime> PickupDateTimes { get; set; }
		int pickupDatesIndex;

		public decimal ServiceCharge { get; set; }
		public decimal Subtotal { get; set; }
		public decimal Taxes { get; set; }
		decimal taxRate;
		public decimal Total { get; set; }

		public DateTime PickupDate { get; set; }

		TimeSpan pickupTime;
		public TimeSpan PickupTime {
			get {
				return pickupTime;
			}
			set {
				SetProperty(ref pickupTime, value);
			}
		}

		string menuHoursText;
		public string MenuHoursText {
			get {
				if (menuHoursText == null)
					menuHoursText = "";
				return menuHoursText;
			}
			set {
				SetProperty(ref menuHoursText, value);
			}
		}

		public CheckoutViewModel () {
			var user = LocalGlobals.User;
			Order = new Order(user.UserId, LocalGlobals.Restaurant.RestaurantId);
			Order.FirstName = user.FirstName;
			Order.LastName = user.LastName;

			Menu = LocalGlobals.Restaurant.Menus.First(m => m.MenuId == Cart.MenuId);
			Restaurant = LocalGlobals.Restaurant;


			BuildOrder();
			FillPickupDates();
			SetEarliestTime();

			pickupDatesIndex = 0;
			ChangeMenuHours(0);
		}

		public void ChangeMenuHours (int pickupDatesIndex) {
			//this.pickupDatesIndex = pickupDatesIndex;
			//PickupDate = PickupDateTimes[pickupDatesIndex];
			var dayOfWeek = (int)DateTime.Now.DayOfWeek;
			var openingTime = Menu.Hours.Opening[dayOfWeek];
			var closingTime = Menu.Hours.Closing[dayOfWeek];
			string menuOpeningHours, menuClosingHours;
			if (openingTime == null)
				menuOpeningHours = "N/A";
			else {
				DateTime time = DateTime.Today.Add(openingTime.Value);
				menuOpeningHours = time.ToString("h:mm tt");
			}

			if (closingTime == null)
				menuClosingHours = "N/A";
			else {
				DateTime time = DateTime.Today.Add(closingTime.Value);
				menuClosingHours = time.ToString("h:mm tt");
			}

			MenuHoursText = string.Format("Menu Hours: {0} - {1}", menuOpeningHours, menuClosingHours);
		}

		public void SetEarliestTime () {
			var dayOfWeek = (int)DateTime.Now.DayOfWeek;
			var openingTime = Menu.Hours.Opening[dayOfWeek].Value;
			var closingTime = Menu.Hours.Closing[dayOfWeek].Value;

			var now = DateTime.Now.TimeOfDay;
			if (now >= openingTime)
				PickupTime = now.Add(new TimeSpan(0, 5, 0));
			else if (now <= openingTime.Subtract(new TimeSpan(0, 10, 0)))
				PickupTime = openingTime;
			else
				PickupTime = now.Add(new TimeSpan(0, 5, 0));

			if (closingTime < PickupTime)
				SetLatestTime();
		}

		public void SetLatestTime () {
			var dayOfWeek = (int)DateTime.Now.DayOfWeek;
			var closingTime = Menu.Hours.Closing[dayOfWeek].Value;

			PickupTime = closingTime;
		}

		void FillPickupDates () {
			PickupDates = new List<string>();
			PickupDateTimes = new List<DateTime>();
			var hours = Restaurant.BusinessHours.Opening;
			var date = DateTime.Now;
			if (hours[(int)date.DayOfWeek] != null) {
				var menuClosingHours = Menu.Hours.Closing[(int)date.DayOfWeek].Value;

				if (menuClosingHours.Subtract(new TimeSpan(0, 15, 0)) > date.TimeOfDay)
					AddDate("Today", date);
			}

			date = date.AddDays(1);
			if (hours[(int)date.DayOfWeek] != null)
				AddDate("Tomorrow", date);

			for (int i = 0; PickupDates.Count < 5; i++) {
				date = date.AddDays(1);
				if (hours[(int)date.DayOfWeek] != null)
					AddDate(string.Format("{0:ddd MMM dd}", date), date);
			}
		}

		void AddDate (string text, DateTime dateTime) {
			PickupDates.Add(text);
			PickupDateTimes.Add(dateTime);
		}

		void BuildOrder () {
			Order.OrderStatus = OrderStatus.Created;
			Order.OrderItems = new List<OrderItem>(Cart.OrderItems);
			Order.MenuId = Cart.MenuId;
			Order.RestaurantId = Cart.RestaurantId;
			ServiceCharge = LocalGlobals.Restaurant.ServiceCharge;
			CalculateOrderCost();
			PickupTime = DateTime.Now.TimeOfDay;
			PickupDate = DateTime.Now;
		}

		void CalculateOrderCost () {
			decimal price = 0m;
			foreach (var orderItem in Order.OrderItems) {
				var itemPrice = orderItem.Product.Price;
				foreach (var addon in orderItem.Addons)
					itemPrice += addon.Price;

				orderItem.ItemPrice = itemPrice;
				price += itemPrice * orderItem.Quantity;
			}

			Order.ServiceCharge = ServiceCharge;
			Subtotal = price;
			Order.OrderSubtotal = Subtotal;
			Order.OrderTaxes = Taxes = (price + ServiceCharge) * (Restaurant.TaxRate / 100m);

			Total = Order.OrderTotal = Subtotal + ServiceCharge + Taxes;
		}
	}
}
