using System;
using System.Collections.Generic;
using Nabbit.Models;
using Nabbit.ViewModels;

namespace NabbitManager.ViewModels {
	public class LiveOrdersViewModel : BaseViewModel {
		List<Order> orders = new List<Order>();
		public List<Order> Orders {
			get {
				return orders;
			}
			set {
				SetProperty(ref orders, value);
			}
		}
		public LiveOrdersViewModel () {
		}
	}
}
