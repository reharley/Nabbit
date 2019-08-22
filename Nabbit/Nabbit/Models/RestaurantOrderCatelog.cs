using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.Models {
	public class RestaurantOrderCatelog {
		public RestaurantOrderCatelog(string restaurantId, int periodDayCount) {
			CatelogId = Guid.NewGuid().ToString();
			PeriodStart = DateTime.Now;
			PeriodEnd = PeriodStart.AddDays(periodDayCount);
			RestaurantId = restaurantId;

			OrderIds = new List<string>();
		}

		public string CatelogId { get; set; }
		public string RestaurantId { get; set; }
		public DateTime PeriodStart { get; set; }
		public DateTime PeriodEnd { get; set; }
		
		public List<string> OrderIds { get; set; }
	}
}
