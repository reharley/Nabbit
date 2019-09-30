using System;
using System.Collections.Generic;
using Nabbit.Models;
using Nabbit.ViewModels;

namespace NabbitManager.ViewModels {
	public class AllOrdersViewModel : BaseViewModel {
		public AllOrdersViewModel () {
		}
		public List<Order> Orders { get; set; }
	}
}
