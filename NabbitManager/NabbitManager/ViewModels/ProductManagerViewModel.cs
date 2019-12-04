using Nabbit.Services;
using Nabbit.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace NabbitManager.ViewModels {
	public class ProductManagerViewModel : BaseViewModel {
		public List<string> pickerCats { get; set; }


		public ProductManagerViewModel () {
			if (LocalGlobals.Restaurant != null) {
				pickerCats = new List<string>();
				pickerCats.Add("All");
				pickerCats.AddRange(LocalGlobals.Restaurant
												.ProductCategories
												.Select(c => c.Name)
												.ToList());
			}
		}

	}
}
