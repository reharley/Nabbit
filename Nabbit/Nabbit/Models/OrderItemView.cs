using System;
using Xamarin.Forms;

namespace Nabbit.Models {
	public class OrderItemView : OrderItem {
		public OrderItemView (OrderItem x) {
			OrderItemId = x.OrderItemId;
			Instructions = x.Instructions;
			Quantity = x.Quantity;
			Product = x.Product;
			Addons = x.Addons;

			ConstructAddonText(x);
			OrderItemPrice = (Product.Price + AddonCost) * Quantity;
		}

		public decimal OrderItemPrice { get; set; }
		public string AddonText { get; set; }
		public decimal AddonCost { get; set; }

		void ConstructAddonText (OrderItem x) {
			if (x.Addons.Count == 0) {
				AddonText = "";
				return;
			}
			var addonGroups = AddonGroupAddons.BuildAddonGroups(x);

			AddonCost = 0m;
			AddonText = "with ";
			var addonCount = 0;
			for (int i = 0; i < addonGroups.Count; i++) {
				if (addonGroups[i].SelectionMode == SelectionMode.Multiple) {
					if (addonGroups[i].DefaultAddons.Count == 0)
						continue;

					for (int j = 0; j < addonGroups[i].DefaultAddons.Count; j++) {
						var addon = addonGroups[i].DefaultAddons[j];
						AddonText += string.Format("{0}-{1} ({2:c})", addonGroups[i].Name, addon.Name, addon.Price);
						AddonCost += addon.Price;
						if (j + 1 != addonGroups[i].DefaultAddons.Count && addonCount + 1 < Addons.Count)
							AddonText += ",\n";
						addonCount++;
					}
				} else {
					var addon = addonGroups[i].DefaultAddon;
					AddonText += string.Format("{0}-{1} ({2:c})", addonGroups[i].Name, addon.Name, addon.Price);
					AddonCost += addon.Price;
					addonCount++;
				}

				if (i + 1 < Addons.Count)
					AddonText += ",\n";
			}
		}
	}
}
