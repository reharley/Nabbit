using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NabbitManager.Services {
	public interface IBluetooth {
		void Start(byte[] bytes);
		void Cancel();
		ObservableCollection<string> PairedDevices();
	}
}
