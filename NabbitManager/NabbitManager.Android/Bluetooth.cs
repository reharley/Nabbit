using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ESCPOS_NET.Emitters;
using Java.IO;
using Java.Util;
using NabbitManager.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(NabbitManager.Droid.Bth))]
namespace NabbitManager.Droid {
	public class Bth : IBluetooth {
		private CancellationTokenSource _ct { get; set; }

		const int RequestResolveError = 1000;

		public Bth() {
		}

		#region IBth implementation


		public async Task Test(byte[] bytes) {
			BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
			if (adapter == null)
				throw new Exception("No Bluetooth adapter found.");

			if (!adapter.IsEnabled)
				throw new Exception("Bluetooth adapter is not enabled.");


			BluetoothDevice device = null;
			foreach (var bd in adapter.BondedDevices) {
				if (bd.Name.ToLower().Contains("printer")) {
					device = bd;
					break;
				}
			}

			if (device == null)
				throw new Exception("Named device not found.");

			var _socket = device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
			_socket.ConnectAsync().Wait();

			// Read data from the device
			//await _socket.InputStream.ReadAsync(buffer, 0, buffer.Length);

			//var cmds = GetLogo();

			// Write data to the device
			//await _socket.OutputStream.WriteAsync(cmds, 0, cmds.Length);
			await _socket.OutputStream.WriteAsync(bytes, 0, bytes.Length);
		}

		byte[] GetLogo () {
			byte[] img = null;

			var asset = Android.App.Application.Context.Assets.Open("nabbit_logo_transparent_outline.bmp");
			using (var memoryStream = new MemoryStream()) {
				asset.CopyTo(memoryStream);
				img = memoryStream.ToArray();
			}

			ICommandEmitter e = new EPSON();
			var receipt = new List<byte[]>();
			receipt.Add(e.Initialize());
			receipt.Add(e.Enable());
			receipt.Add(e.PrintImage(img, true, maxWidth: 256));

			receipt.Add(e.PrintLine());
			receipt.Add(e.PrintLine());
			receipt.Add(e.PartialCutAfterFeed(5));
			return receipt.SelectMany(x => x).ToArray();
		}

		/// <summary>
		/// Start the "reading" loop 
		/// </summary>
		/// <param name="name">Name of the paired bluetooth device (also a part of the name)</param>
		public void Start(byte[] bytes) {
			Test(bytes).Wait();
		}



		private async Task loop(string name, int sleepTime, bool readAsCharArray) {
			
		}

		/// <summary>
		/// Cancel the Reading loop
		/// </summary>
		/// <returns><c>true</c> if this instance cancel ; otherwise, <c>false</c>.</returns>
		public void Cancel() {
			if (_ct != null) {
				System.Diagnostics.Debug.WriteLine("Send a cancel to task!");
				_ct.Cancel();
			}
		}

		public ObservableCollection<string> PairedDevices() {
			BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
			ObservableCollection<string> devices = new ObservableCollection<string>();

			foreach (var bd in adapter.BondedDevices)
				devices.Add(bd.Name);

			return devices;
		}


		#endregion
	}
}