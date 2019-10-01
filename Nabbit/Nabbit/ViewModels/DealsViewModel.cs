using System;
using System.Collections.Generic;
using System.Linq;
using Nabbit.Models;
using Nabbit.Services;

namespace Nabbit.ViewModels {
	public class DealsViewModel : BaseViewModel {
		public Menu Menu { get; set; }
		List<Product> deals;
		public List<Product> Deals {
			get {
				if (deals == null)
					deals = new List<Product>();
				return deals;
			}
			set {
				SetProperty(ref deals, value);
			}
		}

		public DealsViewModel () {
			Menu = LocalGlobals.Restaurant.Menus.First(m => m.Name == "Deals");
			if (Menu != null) {
				var prodCatId = Menu.ProductCategoryIds[0];
				var producCategory = LocalGlobals.Restaurant.ProductCategories
					.First(pc => pc.ProductCategoryId == prodCatId);
				Deals = LocalGlobals.Restaurant.Products
					.Where(p => producCategory.ProductIds.Contains(p.ProductId))
					.ToList();
			} else {
				Deals = new List<Product>();
			}
		}
	}
}
