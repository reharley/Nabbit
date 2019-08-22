using Nabbit.Models;
using Nabbit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Nabbit.ViewModels {
	public class OrderItemEditViewModel : BaseViewModel {
		OrderItem orderItem;
		public OrderItem OrderItem {
			get { return orderItem; }
			set { SetProperty(ref orderItem, value); }
		}

		decimal price;
		public decimal Price {
			get {
				return price;
			}
			set {
				SetProperty(ref price, value);
			}
		}

		public List<AddonGroupAddons> AddonGroups { get; set; }

		public OrderItemEditViewModel(Guid id, bool cartId = false) {
			if (cartId)
				OrderItem = Cart.OrderItems.First(o => o.OrderItemId == id);
			else
				OrderItem = new OrderItem() {
					OrderItemId = Guid.NewGuid(),
					Product = LocalGlobals.Restaurant.Products.FirstOrDefault(p => p.ProductId == id),
					Quantity = 1,
					Instructions = ""
				};

			BuildViewModel();
		}

		public OrderItemEditViewModel(OrderItem orderItem) {
			OrderItem = orderItem;

			BuildViewModel();
		}

		void BuildViewModel() {
			var product = OrderItem.Product;
			Price = product.Price;
			var prodCats = LocalGlobals.Restaurant.ProductCategories
				.Where(pc => pc.ProductIds.Contains(product.ProductId))
				.ToList();
			var addonGroupIds = new List<Guid>();
			if (product.AddonGroupIds != null)
				addonGroupIds.AddRange(product.AddonGroupIds);
			foreach (var prodCat in prodCats)
				addonGroupIds.AddRange(prodCat.AddonGroupIds);

			List<Guid> selectedAddonIds = null;
			if (orderItem.Addons != null)
				selectedAddonIds = orderItem.Addons.Select(a => a.AddonId).ToList();

			var addonGroups = LocalGlobals.Restaurant.AddonGroups.Where(ag => addonGroupIds.Contains(ag.AddonGroupId));
			AddonGroups = new List<AddonGroupAddons>();
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
		}
	}
}
