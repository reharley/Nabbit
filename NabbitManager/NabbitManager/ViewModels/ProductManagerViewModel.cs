using Nabbit.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NabbitManager.ViewModels {
	public class ProductManagerViewModel : BaseViewModel {
		public List<string> pickerCats { get; set; }


		public ProductManagerViewModel() {
		}

	}
}
