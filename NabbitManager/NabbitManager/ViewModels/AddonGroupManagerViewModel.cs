using Nabbit.Models;
using Nabbit.Services;
using Nabbit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NabbitManager.ViewModels {
	public class AddonGroupManagerViewModel : BaseViewModel {
		public List<AddonGroup> AddonGroups { get; set; }

		public AddonGroupManagerViewModel() {
			AddonGroups = LocalGlobals.Restaurant.AddonGroups.OrderBy(ag => ag.Name).ToList();
		}
	}
}
