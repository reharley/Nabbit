using Nabbit.Models;
using Nabbit.Services;
using Nabbit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NabbitManager.ViewModels {
	public class AddonGroupEditViewModel : BaseViewModel {
		public AddonGroup AddonGroup { get; set; }
		public List<string> PickerTypes { get; set; }
		public List<ItemSelector<Addon>> Addons { get; set; }

		public AddonGroupEditViewModel() {
			AddonGroup = new AddonGroup() {
				AddonGroupId = Guid.NewGuid(),
				SelectType = SelectTypes.Check
			};
			BuildViewModel();
		}

		public AddonGroupEditViewModel(Guid addonGroupId) {
			AddonGroup = LocalGlobals.Restaurant.AddonGroups.First(ag => ag.AddonGroupId == addonGroupId);
			BuildViewModel();
		}

		void BuildViewModel() {
			PickerTypes = new List<string>() {
				SelectTypes.Check, SelectTypes.Drop
			};

			Addons = new List<ItemSelector<Addon>>();
			var addons = LocalGlobals.Restaurant.Addons.ToList();
			foreach (var addon in addons) {
				Addons.Add(new ItemSelector<Addon>() {
					Item = addon,
					Selected = AddonGroup.AddonIds.Contains(addon.AddonId)
				});
			}
		}
	}
}
