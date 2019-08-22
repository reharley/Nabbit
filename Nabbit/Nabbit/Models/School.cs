using System;
using System.Collections.Generic;

namespace Nabbit.Models {
	public partial class School : BaseEntity {
		public School() {
			SchoolId = Guid.NewGuid();

			RestaurantIds = new HashSet<Guid>();
		}

		public Guid SchoolId { get; set; }
		public string Name { get; set; }
		public string ShortName { get; set; }
		public string Location { get; set; }
		public string Logo { get; set; }

		public virtual ICollection<Guid> RestaurantIds { get; set; }
	}
}
