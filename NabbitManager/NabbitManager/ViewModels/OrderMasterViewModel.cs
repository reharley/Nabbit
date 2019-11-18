using System;
using Nabbit.ViewModels;

namespace NabbitManager.ViewModels {
	public class OrderMasterViewModel : BaseViewModel {
		string printerStatus = "No";
		public string PrinterStatus {
			get {
				return printerStatus;
			}
			set {
				SetProperty(ref printerStatus, value);
			}
		}

		public OrderMasterViewModel () {
		}
	}
}
