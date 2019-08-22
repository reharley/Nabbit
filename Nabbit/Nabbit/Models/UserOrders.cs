using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.Models {
	public class UserOrders {
		public UserOrders(string userId) {
			UserId = userId;
			OrderIds = new List<string>();
		}
		public string UserId { get; set; }
		public List<string> OrderIds { get; set; }
	}
}
