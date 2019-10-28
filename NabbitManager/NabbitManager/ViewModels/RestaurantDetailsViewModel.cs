using System;
using System.Collections.Generic;
using Com.OneSignal;
using Nabbit.Models;
using Nabbit.Services;
using Nabbit.ViewModels;
using Plugin.SecureStorage;

namespace NabbitManager.ViewModels {
	public class RestaurantDetailsViewModel : BaseViewModel {
		public decimal TaxRate { get; set; }
		public decimal ServiceCharge { get; set; }
		public int PingDelay { get; set; }

		bool usePrinter = false;
		public bool UsePrinter {
			get {
				return usePrinter;
			}
			set {
				SetProperty(ref usePrinter, value);
			}
		}
		public List<ItemSelector<HoursView>> Hours { get; set; }

		public RestaurantDetailsViewModel () {
			BuildModel();
		}

		private void IdsAvailable (string userID, string pushToken) {
			UsePrinter = LocalGlobals.Restaurant.PlayerId.ToString() == userID;
		}

		void BuildModel () {
			PingDelay = LocalGlobals.PingMinuteDelay;
			TaxRate = LocalGlobals.Restaurant.TaxRate;
			ServiceCharge = LocalGlobals.Restaurant.ServiceCharge;

			var installIdString = App.Current.Properties["InstallId"] as string;
			var installId = Guid.Parse(installIdString);
			UsePrinter = LocalGlobals.Restaurant.PrinterId == installId;

			BuildHours();
		}

		void BuildHours () {
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

			if (usePrinter) {
				var installIdString = App.Current.Properties["InstallId"] as string;
				var installId = Guid.Parse(installIdString);
				LocalGlobals.Restaurant.PrinterId = installId;
			}

			LocalGlobals.Restaurant.BusinessHours = hours;
			LocalGlobals.Restaurant.TaxRate = TaxRate;
			LocalGlobals.Restaurant.ServiceCharge = ServiceCharge;
			LocalGlobals.PingMinuteDelay = PingDelay;
		}
	}
}
