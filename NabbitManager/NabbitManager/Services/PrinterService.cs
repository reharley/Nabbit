using ESCPOS_NET.Emitters;
using Nabbit.Models;
using Nabbit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NabbitManager.Services {
	public static class PrinterService {
		public static void Printer(Order order) {
			ICommandEmitter e = new EPSON();
			var receipt = new List<byte[]>();
			receipt.Add(e.Initialize());
			receipt.Add(e.Enable());

			receipt.Add(e.PrintLine());
			receipt.Add(e.PrintLine());
			receipt.Add(e.PrintLine());
			receipt.Add(e.PrintLine());
			receipt.Add(e.PrintLine("Nabbit Order"));
			receipt.Add(e.PrintLine($"{LocalGlobals.Restaurant.Name}"));
			receipt.Add(e.PrintLine());
			receipt.Add(e.PrintLine());
			receipt.Add(e.PrintLine($"First Name: {order.FirstName}"));
			receipt.Add(e.PrintLine($"Last Name: {order.LastName}"));
			receipt.Add(e.PrintLine(string.Format("Pickup Time: {0:ddd, MMM d, hh:mm tt}", order.PickupTime)));
			receipt.Add(e.PrintLine());
			receipt.Add(e.PrintLine());
			foreach (var item in order.OrderItems) {
				receipt.Add(e.Print($"{item.Product.Name}"));
				receipt.Add(e.Print($"\t\tx{item.Quantity}"));
				receipt.Add(e.PrintLine());
				foreach (var addon in item.Addons) {
					receipt.Add(e.PrintLine($"{addon.Name}"));
				}
			}

			receipt.Add(e.PrintLine());
			receipt.Add(e.PrintLine());
			receipt.Add(e.PartialCutAfterFeed(5));

			IBluetooth service = DependencyService.Get<IBluetooth>();
			service.Start(receipt.SelectMany(x => x).ToArray());
		}
	}
}
