using Nabbit.Views;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Nabbit {
	public partial class AppShell : Xamarin.Forms.Shell {
		Dictionary<string, Type> routes = new Dictionary<string, Type>();

		public Dictionary<string, Type> Routes { get { return routes; } }
		public AppShell() {
			InitializeComponent();
			RegisterRoutes();
		}


		void RegisterRoutes() {
			routes.Add("orderitem", typeof(OrderItemEditPage));
			routes.Add("menu", typeof(MenuPage));
			routes.Add("orderHistory", typeof(OrderHistoryPage));
			routes.Add("paymentMethods", typeof(PaymentMethodsPage));

			foreach (var item in routes) {
				Routing.RegisterRoute(item.Key, item.Value);
			}
		}
	}
}
