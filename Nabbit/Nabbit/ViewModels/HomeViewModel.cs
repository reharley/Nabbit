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
		public string MenuOpeningHours { get; set; }
		public string MenuClosingHours { get; set; }

		List<List<ProductCategoryProducts>> menus;
		int menuIndex;


		List<ProductCategoryProducts> _pcProducts;
		public List<ProductCategoryProducts> pcProducts {
			get {
				if (_pcProducts == null)
					_pcProducts = new List<ProductCategoryProducts>();

				return _pcProducts;
			}
			set {
				SetProperty(ref _pcProducts, value);
			}
		}

		public List<Models.Menu> Menus { get; set; }
		public Models.Menu SelectedMenu { get; set; }

		public HomeViewModel () {
			menus = new List<List<ProductCategoryProducts>>();
			BuildCategoryProducts();
			menuIndex = 0;
			pcProducts = menus[menuIndex];
			SelectedMenu = Menus[menuIndex];
			var dayOfWeek = (int)DateTime.Today.DayOfWeek;
			var openingTime = SelectedMenu.Hours.Opening[dayOfWeek];
			var closingTime = SelectedMenu.Hours.Closing[dayOfWeek];

			if (openingTime == null)
				MenuOpeningHours = "N/A";
			else {
				DateTime time = DateTime.Today.Add(openingTime.Value);
				MenuOpeningHours = time.ToString("h:mm tt");
			}

			if (closingTime == null)
				MenuClosingHours = "N/A";
			else {
				DateTime time = DateTime.Today.Add(closingTime.Value);
				MenuClosingHours = time.ToString("h:mm tt");
			}
		}

		public Models.Menu GetMenuName () {
			return Menus[menuIndex];
		}

		public void ChangeMenu (Models.Menu menu) {
			menuIndex = Menus.IndexOf(menu);
			pcProducts = menus[menuIndex];
			SelectedMenu = Menus[menuIndex];
		}

		public void BuildCategoryProducts () {
			if (LocalGlobals.Restaurant != null) {
				Menus = new List<Models.Menu>();
				foreach (var menu in LocalGlobals.Restaurant.Menus) {
					var pcProductsTmp = new List<ProductCategoryProducts>();
					Menus.Add(menu);
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

						pcProductsTmp.Add(pcProduct);
					}

					menus.Add(pcProductsTmp);
				}
			}
		}

		public void ChangeMenu (SwipeDirection swipeDirection) {
			if (swipeDirection == SwipeDirection.Left) {
				if (menuIndex > 0) {
					pcProducts = menus[--menuIndex];
				}
			} else if (swipeDirection == SwipeDirection.Right) {
				if (menuIndex < menus.Count - 1) {
					pcProducts = menus[menuIndex++];
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

		public class MenuView {
			public MenuView () {
				Menus = new List<ProductCategoryProducts>();
			}

			public List<ProductCategoryProducts> Menus { get; set; }
		}
	}
}
