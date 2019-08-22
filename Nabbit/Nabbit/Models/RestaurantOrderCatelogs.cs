using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.Models {
	public class RestaurantOrderCatelogs {
		public RestaurantOrderCatelogs() {
			PeriodDayCount = 14;
			CatelogIds = new List<string>();
		}

		public int PeriodDayCount { get; set; }
		public List<string> CatelogIds { get; set; }
	}
}
