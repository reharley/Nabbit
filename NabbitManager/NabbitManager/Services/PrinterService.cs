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
		static IBluetooth service;

		public static async Task PrinterAsync (Order order) {
			string UID;
			string CreationDate;
			string PickupTime;
			decimal ServiceCharge;

			List<OrderItemView> orderItems;

			orderItems = order.OrderItems.Select(x => new OrderItemView(x)).ToList();

			var uid = order.UserId.ToString();
			UID = uid.Substring(uid.Length - 7).ToUpper().Insert(3, "-");

			CreationDate = order.CreationDate.ToString("MM/dd/yy hh:mm tt");
			PickupTime = order.PickupTime.ToString("MM/dd/yy hh:mm tt");

			ServiceCharge = order.OrderTotal - order.OrderSubtotal - order.OrderTaxes;

			ICommandEmitter e = new EPSON();
			var receipt = new List<byte[]>();
			receipt.Add(e.Initialize());
			receipt.Add(e.Enable());
			receipt.Add(e.SetStyles(PrintStyle.DoubleHeight | PrintStyle.DoubleWidth | PrintStyle.Bold));
			receipt.Add(e.CenterAlign());
			receipt.Add(e.PrintLine("Nabbit Order"));
			receipt.Add(e.PrintLine($"{LocalGlobals.Restaurant.Name}"));
			receipt.Add(e.PrintLine($"UID: {UID}"));
			receipt.Add(e.SetStyles(PrintStyle.None));
			receipt.Add(e.LeftAlign());
			receipt.Add(e.PrintLine());
			receipt.Add(e.PrintLine("Order For"));
			receipt.Add(e.Print($"{order.FirstName} "));
			receipt.Add(e.PrintLine($"{order.LastName}"));
			receipt.Add(e.PrintLine("Order Creation\t\tPickup Time"));
			receipt.Add(e.Print(order.CreationDate.ToString("MM/dd/yy hh:mm tt")));
			receipt.Add(e.Print("\t"));
			receipt.Add(e.Print(order.PickupTime.ToString("MM/dd/yy hh:mm tt")));
			receipt.Add(e.PrintLine());
			receipt.Add(e.PrintLine());
			receipt.Add(e.SetStyles(PrintStyle.DoubleHeight | PrintStyle.DoubleWidth | PrintStyle.Bold));
			receipt.Add(e.PrintLine("Items:"));
			foreach (var item in orderItems) {
				receipt.Add(e.SetStyles(PrintStyle.None));
				receipt.Add(e.PrintLine("________________________________________________"));
				receipt.Add(e.SetStyles(PrintStyle.DoubleHeight | PrintStyle.DoubleWidth | PrintStyle.Bold));
				receipt.Add(e.Print($"{item.Product.Name}"));
				receipt.Add(e.SetStyles(PrintStyle.None));
				receipt.Add(e.SetStyles(PrintStyle.Bold));
				receipt.Add(e.Print($"\t\t{item.OrderItemPrice}"));
				receipt.Add(e.PrintLine());
				receipt.Add(e.PrintLine());
				receipt.Add(e.SetStyles(PrintStyle.DoubleHeight | PrintStyle.DoubleWidth | PrintStyle.Bold));
				receipt.Add(e.PrintLine($"Quantity: {item.Quantity}"));
				receipt.Add(e.PrintLine());

				if (item.Instructions != null || item.Instructions != "") {
					receipt.Add(e.SetStyles(PrintStyle.None));
					receipt.Add(e.SetStyles(PrintStyle.DoubleHeight));
					receipt.Add(e.PrintLine($"Special Instructions:"));
					receipt.Add(e.SetStyles(PrintStyle.None));
					receipt.Add(e.PrintLine($"{item.Instructions}"));
					receipt.Add(e.PrintLine());
				}

				if (item.Addons.Count == 0) {
					receipt.Add(e.PrintLine("Addons"));
					receipt.Add(e.SetStyles(PrintStyle.DoubleHeight));
					receipt.Add(e.SetStyles(PrintStyle.None));

					foreach (var addonLine in item.AddonText.Split('\n')) {
						receipt.Add(e.PrintLine($"{addonLine}"));
					}
				}
			}

			receipt.Add(e.SetStyles(PrintStyle.None));
			receipt.Add(e.PrintLine("________________________________________________"));
			receipt.Add(e.PrintLine());
			receipt.Add(e.PrintLine());
			receipt.Add(e.PrintLine());

			var orderSubtotal = string.Format("{0:C}", order.OrderSubtotal);
			var serviceCharge = string.Format("{0:C}", order.ServiceCharge);
			var taxes = string.Format("{0:C}", order.OrderTaxes);
			var orderTotal = string.Format("{0:C}", order.OrderTotal);

			receipt.Add(e.Print("Order Subtitle "));
			receipt.Add(e.Print("........................"));
			receipt.Add(e.RightAlign());
			receipt.Add(e.PrintLine(orderSubtotal));

			receipt.Add(e.LeftAlign());
			receipt.Add(e.Print("Service Charge "));
			receipt.Add(e.Print("........................"));
			receipt.Add(e.RightAlign());
			receipt.Add(e.PrintLine(orderSubtotal));

			receipt.Add(e.LeftAlign());
			receipt.Add(e.Print("Taxes "));
			receipt.Add(e.Print("........................"));
			receipt.Add(e.RightAlign());
			receipt.Add(e.PrintLine(taxes));

			receipt.Add(e.LeftAlign());
			receipt.Add(e.Print("Order Total "));
			receipt.Add(e.Print("........................"));
			receipt.Add(e.RightAlign());
			receipt.Add(e.PrintLine(orderTotal));

			receipt.Add(e.PrintLine());
			receipt.Add(e.PartialCutAfterFeed(2));

			if (service == null)
				service = DependencyService.Get<IBluetooth>();
			//service.Start(receipt);
			await service.Start(receipt.SelectMany(x => x).ToArray());
		}
	}
}
