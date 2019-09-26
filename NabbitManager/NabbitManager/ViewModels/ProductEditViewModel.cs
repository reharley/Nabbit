using Nabbit.Models;
using Nabbit.Services;
using Nabbit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NabbitManager.ViewModels {
	public class ProductEditViewModel : BaseViewModel {
		public Product Product { get; set; }
		public List<ItemSelector<AddonGroup>> AddonGroups { get; set; }

		public ProductEditViewModel() {
			Product = new Product() {
				ProductId = Guid.NewGuid()
			};

			BuildViewModel();
		}

		public ProductEditViewModel(Guid productId) {
			Product = LocalGlobals.Restaurant.Products.FirstOrDefault(p => p.ProductId == productId);

			BuildViewModel();
		}

		void BuildViewModel() {
			if (Product.AddonGroupIds == null)
				Product.AddonGroupIds = new List<Guid>();

			AddonGroups = new List<ItemSelector<AddonGroup>>();
			var addonGroups = LocalGlobals.Restaurant.AddonGroups.ToList();
			foreach (var group in addonGroups) {
				bool selected = Product.AddonGroupIds.Contains(group.AddonGroupId);
				AddonGroups.Add(new ItemSelector<AddonGroup>() {
					Item = group,
					Selected = selected
				});
			}
		}

		public void SaveModel () {
			var addonGroupIds = new List<Guid>();
			foreach (var item in AddonGroups) {
				if (item.Selected)
					addonGroupIds.Add(item.Item.AddonGroupId);
			}

			Product.AddonGroupIds = addonGroupIds;
		}
	}
}
