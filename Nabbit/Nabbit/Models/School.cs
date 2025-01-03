﻿using System;
using System.Collections.Generic;

namespace Nabbit.Models {
	public class School : BaseEntity {
		public School() {
			SchoolId = Guid.NewGuid();

			RestaurantIds = new List<Guid>();
		}

		public Guid SchoolId { get; set; }
		public string Name { get; set; }
		public string ShortName { get; set; }
		public string Location { get; set; }
		public string Logo { get; set; }

		public List<Guid> RestaurantIds { get; set; }
	}
}
