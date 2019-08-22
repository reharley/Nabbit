using System;
using System.Collections.Generic;

namespace Nabbit.Models {
	public partial class Restaurant : BaseEntity {
		public Restaurant(Guid schoolId, string name) {
			RestaurantId = Guid.NewGuid();
			SchoolId = schoolId;
			Name = name;

			Addons = new HashSet<Addon>();
			AddonGroups = new HashSet<AddonGroup>();
			BusinessHours = new HashSet<BusinessHours>();
			Menus = new HashSet<Menu>();
			Products = new HashSet<Product>();
			ProductCategories = new HashSet<ProductCategory>();
		}

		public Restaurant() {}

		public Guid RestaurantId { get; set; }
		public Guid SchoolId { get; set; }
		public string Name { get; set; }
		public int Version { get; set; }

		public virtual ICollection<Addon> Addons { get; set; }
		public virtual ICollection<AddonGroup> AddonGroups { get; set; }
		public virtual ICollection<BusinessHours> BusinessHours { get; set; }
		public virtual ICollection<Menu> Menus { get; set; }
		public virtual ICollection<Product> Products { get; set; }
		public virtual ICollection<ProductCategory> ProductCategories { get; set; }
	}
}
