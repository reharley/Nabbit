﻿using Nabbit.Models;
using Nabbit.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Nabbit.ViewModels {
	public class CartViewModel : BaseViewModel {
		public CartViewModel () {
			RefreshCart();
		}

		decimal orderTotal;
		public decimal OrderTotal {
			get {
				return orderTotal;
			}
			set {
				SetProperty(ref orderTotal, value);
			}
		}

		public List<OrderItemView> cart;
		public List<OrderItemView> Cart {
			get {
				return cart;
			}
			set {
				SetProperty(ref cart, value);
			}
		}

		public void RefreshCart () {
			if (Cart != null)
				Cart.Clear();
			Cart = Nabbit.Models.Cart.OrderItems.Select(x => new OrderItemView(x)).ToList();
			OrderTotal = Cart.Sum(item => item.OrderItemPrice);
		}
	}
}
