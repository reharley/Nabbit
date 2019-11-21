using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Nabbit.Services;
using Nabbit.Views;
using Nabbit.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Nabbit.Services.LogOn;

namespace Nabbit {
	public partial class App : Application {
		public static string ApiEndpoint = "https://nabbit.azurewebsites.net/hello";

		public App () {
			InitializeComponent();
			DependencyService.Register<B2CAuthenticationService>();
			MainPage = new AppShell();
		}

		protected override void OnStart () {
			Cart.OrderItems = new List<OrderItem>();

			AppCenter.Start("android=e0554b49-7cd0-4e03-992d-b7fe395fbc19;" +
				"ios=2297da99-c2fd-4ead-abfb-930c804e2b79;",
				  typeof(Analytics), typeof(Crashes));
		}

		protected override void OnSleep () {
		}

		protected override void OnResume () {
			if (Cart.OrderItems == null)
				Cart.OrderItems = new List<OrderItem>();
		}
	}
}
