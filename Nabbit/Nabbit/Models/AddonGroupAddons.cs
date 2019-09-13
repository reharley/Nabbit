using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nabbit.Services;
using Xamarin.Forms;

namespace Nabbit.Models {
	public class AddonGroupAddons {
		public Guid AddonGroupId { get; set; }
		public string Name { get; set; }
		public string SelectType { get; set; }
		public SelectionMode SelectionMode { get; set; }
		public Addon DefaultAddon { get; set; }
		public List<Addon> DefaultAddons { get; set; }

		public List<Addon> Addons { get; set; }

		public static List<AddonGroupAddons> BuildAddonGroups (OrderItem orderItem) {
			var prodCats = LocalGlobals.Restaurant.ProductCategories
				.Where(pc => pc.ProductIds.Contains(orderItem.Product.ProductId))
				.ToList();
			var addonGroupIds = new List<Guid>();
			if (orderItem.Product.AddonGroupIds != null)
				addonGroupIds.AddRange(orderItem.Product.AddonGroupIds);
			foreach (var prodCat in prodCats)
				addonGroupIds.AddRange(prodCat.AddonGroupIds);

			List<Guid> selectedAddonIds = null;
			if (orderItem.Addons != null)
				selectedAddonIds = orderItem.Addons.Select(a => a.AddonId).ToList();

			var addonGroups = LocalGlobals.Restaurant.AddonGroups.Where(ag => addonGroupIds.Contains(ag.AddonGroupId));
			var AddonGroups = new List<AddonGroupAddons>();
			foreach (var addonGroup in addonGroups) {
				var selectionMode = addonGroup.SelectType == SelectTypes.Check ? SelectionMode.Multiple : SelectionMode.Single;
				var addons = LocalGlobals.Restaurant.Addons.Where(a => addonGroup.AddonIds.Contains(a.AddonId)).ToList();
				Addon defaultAddon = null;
				var defaultAddons = new List<Addon>();
				if (addonGroup.SelectType != SelectTypes.Check)
					defaultAddon = addons.First(a => a.Price == 0m) ?? addons[0];

				if (orderItem.Addons == null)
					orderItem.Addons = new List<Addon>();
				else if (orderItem.Addons.Count > 0) {
					defaultAddons.AddRange(addons
						.Where(a => selectedAddonIds.Contains(a.AddonId))
						.Where(a => addonGroup.AddonIds.Contains(a.AddonId)));
				}

				AddonGroups.Add(new AddonGroupAddons() {
					AddonGroupId = addonGroup.AddonGroupId,
					Name = addonGroup.Name,
					SelectType = addonGroup.SelectType,
					Addons = addons,
					DefaultAddon = defaultAddon,
					DefaultAddons = defaultAddons,
					SelectionMode = selectionMode
				});
			}

			return AddonGroups;
		}
	}
}
