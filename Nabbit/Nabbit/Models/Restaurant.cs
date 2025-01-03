﻿using System;
using System.Collections.Generic;

namespace Nabbit.Models {
	public class Restaurant : BaseEntity {
		public Restaurant (Guid schoolId, string name) {
			RestaurantId = Guid.NewGuid();
			SchoolId = schoolId;
			Name = name;

			Addons = new List<Addon>();
			AddonGroups = new List<AddonGroup>();
			Menus = new List<Menu>();
			Products = new List<Product>();
			ProductCategories = new List<ProductCategory>();
		}

		public Restaurant () { }

		public Guid RestaurantId { get; set; }
		public Guid SchoolId { get; set; }
		/// <summary>The id used to ping the printer through OneSignal</summary>
		public Guid PlayerId { get; set; }
		/// <summary>Id used for determing which device is a printer</summary>
		public Guid PrinterId { get; set; }

		public string Name { get; set; }
		public int Version { get; set; }
		public Hours BusinessHours { get; set; }
		public decimal TaxRate { get; set; }
		public decimal ServiceCharge { get; set; }
		public TimeSpan PickupDelay { get; set; }
		public bool IsActive { get; set; }
		public bool UpdateRestaurant { get; set; }
		public DateTime LastPing { get; set; }

		public List<Addon> Addons { get; set; }
		public List<AddonGroup> AddonGroups { get; set; }
		public List<Menu> Menus { get; set; }
		public List<Product> Products { get; set; }
		public List<ProductCategory> ProductCategories { get; set; }

		public bool IsOpen () {
			var now = DateTime.Now.TimeOfDay;
			var dayOfWeek = (int)DateTime.Now.DayOfWeek;
			var hours = BusinessHours;
			var openingHours = hours.Opening[dayOfWeek];
			var closingHours = hours.Closing[dayOfWeek];

			if (openingHours != null && closingHours != null) {
				if (openingHours.Value <= now && now <= closingHours.Value)
					return true;
			}

			return false;
		}
	}
}
