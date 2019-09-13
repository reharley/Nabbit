using Nabbit.Models;
using Nabbit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Nabbit.ViewModels {
	public class HomeViewModel : BaseViewModel {
		List<ProductCategoryProducts> _pcProducts;
		public List<ProductCategoryProducts> pcProducts {
			get {
				return _pcProducts;
			}
			set {
				SetProperty(ref _pcProducts, value);
			}
		}

		public HomeViewModel () {
			_pcProducts = new List<ProductCategoryProducts>();
			BuildCategoryProducts();
		}

		public void BuildCategoryProducts () {
			if (LocalGlobals.Restaurant != null) {
				foreach (var menu in LocalGlobals.Restaurant.Menus) {
					foreach (var category in LocalGlobals.Restaurant.ProductCategories
													.Where(pc => menu.ProductCategoryIds.Contains(pc.ProductCategoryId))
													.OrderBy(pc => pc.Rank)) {
						var products = LocalGlobals.Restaurant.Products.Where(p => category.ProductIds.Contains(p.ProductId))
										.Select(x => new ProductView(x))
										.ToList();
						products[0].SeparatorOn = false;
						var pcProduct = new ProductCategoryProducts {
							CategoryName = category.Name,
							MenuName = menu.Name,
							MenuDescription = menu.Description,
							Products = products
						};

						pcProducts.Add(pcProduct);
					}
				}
			}
		}

		public class ProductView : Product {
			public ProductView (Product x) {
				ProductId = x.ProductId;
				Name = x.Name;
				Price = x.Price;
				Description = x.Description;
				AddonGroupIds = x.AddonGroupIds;
				SeparatorOn = true;
			}
			public bool SeparatorOn { get; set; }
		}

		public class ProductCategoryProducts {
			public string CategoryName { get; set; }
			public string MenuName { get; set; }
			public string MenuDescription { get; set; }
			public bool SeparatorOn { get; set; }

			public List<ProductView> Products { get; set; }
		}
	}
}
