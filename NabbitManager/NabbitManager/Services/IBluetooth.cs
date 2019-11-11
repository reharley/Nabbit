using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace NabbitManager.Services {
	public interface IBluetooth {
		Task Start (byte[] bytes);
		void Cancel ();
		ObservableCollection<string> PairedDevices ();
	}
}
