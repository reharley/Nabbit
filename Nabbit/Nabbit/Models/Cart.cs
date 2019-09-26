using Nabbit.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.Models {
	public static class Cart {
		public static List<OrderItem> OrderItems { get; set; }
		public static Guid RestaurantId { get; set; }
		public static Guid MenuId { get; set; }
	}
}
