using System;
using System.Linq;
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
			if (x.Addons == null)
				return;

			if (x.Addons.Count == 0) {
				AddonText = "";
				return;
			}
			var addonGroups = AddonGroupAddons.BuildAddonGroups(x);

			var addons = x.Addons.ToList();
			var addonIds = addons.Select(a => a.AddonId);
			AddonCost = 0m;
			AddonText = "with ";
			var addonCount = 0;
			for (int i = 0; i < addonGroups.Count; i++) {
				var groupAddonIds = addonGroups[i].Addons.Select(a => a.AddonId);
				for (int j = 0; j < addons.Count; j++) {
					if (groupAddonIds.Contains(addons[j].AddonId)) {
						var addon = addons[j];
						AddonText += string.Format("{0}-{1} ({2:c})", addonGroups[i].Name, addon.Name, addon.Price);
						AddonCost += addon.Price;
						addonCount++;
						addons.RemoveAt(j);
					}
				}
				if (i + 1 < Addons.Count)
					AddonText += ",\n";
			}
		}
	}
}
