using System;
using System.Collections.Generic;
using Nabbit.Models;
using Nabbit.Services;

namespace NabbitManager.ViewModels {
	public class RestaurantDetailsViewModel : BaseView {
		public decimal TaxRate { get; set; }
		public List<ItemSelector<HoursView>> Hours { get; set; }

		public RestaurantDetailsViewModel () {
			BuildModel();
		}

		void BuildModel () {
			TaxRate = LocalGlobals.Restaurant.TaxRate;
			Hours = new List<ItemSelector<HoursView>>();
			var hours = LocalGlobals.Restaurant.BusinessHours;
			if (hours == null) {
				hours = new Hours();
				hours.BuildHours();
			}
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
			var hours = new Hours();
			hours.BuildHours();
			for (int i = 0; i < Hours.Count; i++) {
				if (Hours[i].Selected) {
					hours.Opening[i] = Hours[i].Item.Open;
					hours.Closing[i] = Hours[i].Item.Close;
				} else {
					hours.Opening[i] = null;
					hours.Closing[i] = null;
				}
			}

			LocalGlobals.Restaurant.BusinessHours = hours;
		}
	}
}
