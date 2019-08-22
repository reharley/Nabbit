using Nabbit.Models;
using Nabbit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nabbit.ViewModels {
	public class ProductDetailsViewModel {
		public Product Product { get; set; }
		public List<AddonGroupAddons> AddonGroups { get; set; }

		public ProductDetailsViewModel(string productId) {
			Product = LocalGlobals.Restaurant.Products.FirstOrDefault(p => p.ProductId.ToString() == productId);
			var prodCats = LocalGlobals.Restaurant.ProductCategories
				.Where(pc => pc.ProductIds.Contains(Product.ProductId))
				.ToList();
			var addonIds = new List<Guid>();
			if (Product.AddonGroupIds != null)
				addonIds.AddRange(Product.AddonGroupIds);
			foreach (var prodCat in prodCats)
				addonIds.AddRange(prodCat.AddonGroupIds);

			var addons = LocalGlobals.Restaurant.Addons.Where(a => addonIds.Contains(a.AddonId));
			var addGroups = LocalGlobals.Restaurant.AddonGroups.Where(ag => addonIds.Contains(ag.AddonGroupId));
			AddonGroups = new List<AddonGroupAddons>();
			foreach (var addonGroup in addGroups) {
				AddonGroups.Add(new AddonGroupAddons() {
					AddonGroupId = addonGroup.AddonGroupId,
					Name = addonGroup.Name,
					SelectType = addonGroup.SelectType,
					//Addons = addons.Where(a => addonGroup.AddonIds.Contains(a.AddonId)).ToList()
				});
			}
		}
	}
}
