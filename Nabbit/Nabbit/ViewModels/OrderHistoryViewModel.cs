using Nabbit.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.ViewModels {
	public class OrderHistoryViewModel : BaseViewModel {
		public List<Order> Orders { get; set; }
	}
}
