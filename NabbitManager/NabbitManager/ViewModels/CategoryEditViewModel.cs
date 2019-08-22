using Nabbit.Models;
using Nabbit.Services;
using Nabbit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NabbitManager.ViewModels {
	public class CategoryEditViewModel : BaseViewModel {
		public ProductCategory ProductCategory { get; set; }
		public List<ItemSelector<Product>> Products { get; set; }
		public List<ItemSelector<AddonGroup>> AddonGroups { get; set; }

		public CategoryEditViewModel() {
			ProductCategory = new ProductCategory() {
				ProductCategoryId = Guid.NewGuid(),
				Rank = LocalGlobals.Restaurant.ProductCategories.Count + 1
			};
			BuildViewModel();
		}

		public CategoryEditViewModel(Guid categoryId) {
			ProductCategory = LocalGlobals.Restaurant.ProductCategories
								.FirstOrDefault(pc => pc.ProductCategoryId == categoryId);
			BuildViewModel();
		}

		void BuildViewModel() {
			var products = LocalGlobals.Restaurant.Products.ToList();

			Products = new List<ItemSelector<Product>>();
			foreach (var prod in products) {
				bool selected = ProductCategory.ProductIds.Contains(prod.ProductId);
				Products.Add(new ItemSelector<Product>() {
					Item = prod,
					Selected = selected
				});
			}

			AddonGroups = new List<ItemSelector<AddonGroup>>();
			var addonGroups = LocalGlobals.Restaurant.AddonGroups.ToList();
			foreach (var group in addonGroups) {
				bool selected = ProductCategory.AddonGroupIds.Contains(group.AddonGroupId);
				AddonGroups.Add(new ItemSelector<AddonGroup>() {
					Item = group,
					Selected = selected
				});
			}
		}
	}
}
