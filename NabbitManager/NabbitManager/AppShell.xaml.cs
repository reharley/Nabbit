using NabbitManager.Views;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace NabbitManager {
	public partial class AppShell : Xamarin.Forms.Shell {
		Dictionary<string, Type> routes = new Dictionary<string, Type>();

		public AppShell() {
			InitializeComponent();
			RegisterRoutes();
		}
		void RegisterRoutes() {
			routes.Add("addonmanager", typeof(AddonManagerPage));
			routes.Add("addongroupmanager", typeof(AddonGroupManagerPage));
			routes.Add("productmanager", typeof(ProductManagerPage));
			routes.Add("categorymanager", typeof(CategoryManagerPage));
			routes.Add("menumanager", typeof(MenuManagerPage));
			routes.Add("allOrders", typeof(AllOrdersPage));
			routes.Add("liveOrders", typeof(LiveOrdersPage));

			foreach (var item in routes) {
				Routing.RegisterRoute(item.Key, item.Value);
			}
		}
	}
}
