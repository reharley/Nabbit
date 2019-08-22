using Nabbit.Models;
using Nabbit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NabbitManager.ViewModels {
	public class OrderDetailViewModel {
		public Order Order { get; set; }
		public List<OrderItem> OrderItems { get; set; }
		public List<Product> Products { get; set; }
		public decimal Subtotal { get; set; }
		public decimal Total { get; set; }

		public OrderDetailViewModel(string orderId) {
			//Order = LocalGlobals.Restaurant.Order.FirstOrDefault(o => o.Id == orderId);
			//Order = OrderRepo.Get().FirstOrDefault(o => o.Id == orderId);
			//OrderItems = OrderItemRepo.Get().Where(o => o.OrderId == Order.Id).ToList();
			//List<string> productIds = OrderItems.Select(o => o.ProductId).ToList();
			//List<Product> orderProducts = ProductRepo.Get().Where(p => productIds.Contains(p.Id)).ToList();

			//decimal total = 0M, taxRate = 0.065M;
			//foreach (var item in OrderItems) {
			//	item.Product = orderProducts.FirstOrDefault(p => p.Id == item.ProductId);
			//	total += item.Product.Price * item.Quantity;
			//}
			
			//Subtotal = total;
			//Total = total + total * taxRate;
		}
	}
}
