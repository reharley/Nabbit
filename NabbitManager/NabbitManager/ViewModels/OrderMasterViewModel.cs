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

		string connectionStatus = "OFFLINE";
		public string ConnectionStatus {
			get {
				return connectionStatus;
			}
			set {
				SetProperty(ref connectionStatus, value);
			}
		}

		string openStatus= "Closed";
		public string OpenStatus {
			get {
				return openStatus;
			}
			set {
				SetProperty(ref openStatus, value);
			}
		}

		public OrderMasterViewModel () {
		}
	}
}
