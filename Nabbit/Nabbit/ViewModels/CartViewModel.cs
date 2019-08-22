using Nabbit.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.ViewModels {
	public class CartViewModel : BaseViewModel {
		public List<OrderItem> cart;
		public List<OrderItem> Cart {
			get {
				return cart;
			}
			set {
				SetProperty(ref cart, value);
			}
		}
	}
}
