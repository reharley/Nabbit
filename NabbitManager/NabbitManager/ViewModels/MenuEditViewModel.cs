using System;
using System.Collections.Generic;
using System.Linq;
using Nabbit.Models;
using Nabbit.Services;

namespace NabbitManager.ViewModels {
	public class MenuEditViewModel {
		public Menu Menu { get; set; }
		public List<ItemSelector<ProductCategory>> ProductCategories { get; set; }
		public List<ItemSelector<HoursView>> Hours { get; set; }

		public MenuEditViewModel () {
			Menu = new Menu() {
				MenuId = Guid.NewGuid()
			};

			BuildViewModel();
		}

		public MenuEditViewModel(Guid menuId) {
			Menu = LocalGlobals.Restaurant.Menus.FirstOrDefault(m => m.MenuId == menuId);

			BuildViewModel();
		}

		void BuildViewModel () {
			if (Menu.ProductCategoryIds == null)
				Menu.ProductCategoryIds = new List<Guid>();

			ProductCategories = new List<ItemSelector<ProductCategory>>();
			var productCategories = LocalGlobals.Restaurant.ProductCategories;
			foreach (var cat in productCategories) {
				bool selected = Menu.ProductCategoryIds.Contains(cat.ProductCategoryId);
				ProductCategories.Add(new ItemSelector<ProductCategory>() {
					Item = cat,
					Selected = selected
				});
			}

			Hours = new List<ItemSelector<HoursView>>();
			if (Menu.Hours == null)
				Menu.Hours = new Hours();

			var hours = Menu.Hours;
			var days = Nabbit.Models.Hours.BuildDays();
			for (int i = 0; i < hours.Opening.Count; i++) {
				TimeSpan opening, closing;
				bool open = true;

				if (hours.Opening[i] == null) {
					open = false;
					opening = new TimeSpan();
					closing = new TimeSpan();
				} else {
					opening = hours.Opening[i].Value;
					closing = hours.Closing[i].Value;
				}

				var hoursView = new HoursView() {
					Day = days[i].Substring(0, 3),
					Open = opening,
					Close = closing
				};

				Hours.Add(new ItemSelector<HoursView>() {
					Item = hoursView,
					Selected = open
				});
			}
		}

		public void SaveModel () {
			var productCatIds = new List<Guid>();
			foreach (var item in ProductCategories) {
				if (item.Selected)
					productCatIds.Add(item.Item.ProductCategoryId);
			}

			var hours = new Hours();
			for (int i = 0; i < Hours.Count; i++) {
				if (Hours[i].Selected) {
					hours.Opening[i] = Hours[i].Item.Open;
					hours.Closing[i] = Hours[i].Item.Close;
				} else {
					hours.Opening[i] = null;
					hours.Closing[i] = null;
				}
			}

			Menu.ProductCategoryIds = productCatIds;
			Menu.Hours = hours;
		}
	}

	public class HoursView {
		public string Day { get; set; }
		public TimeSpan Open { get; set; }
		public TimeSpan Close { get; set; }
	}
}
