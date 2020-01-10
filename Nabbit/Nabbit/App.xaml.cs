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
using Microsoft.Identity.Client;

namespace Nabbit {
	public partial class App : Application {
		public static IPublicClientApplication AuthenticationClient { get; private set; }
		public static object UIParent { get; set; } = null;

		public App () {
			InitializeComponent();
			MainPage = new AppShell();
		}

		protected override void OnStart () {
			Cart.OrderItems = new List<OrderItem>();

			AppCenter.Start("android=e0554b49-7cd0-4e03-992d-b7fe395fbc19;" +
				"ios=2297da99-c2fd-4ead-abfb-930c804e2b79;",
				  typeof(Analytics), typeof(Crashes));

			AuthenticationClient = PublicClientApplicationBuilder.Create(ADConstants.ClientId)
			.WithIosKeychainSecurityGroup(ADConstants.IosKeychainSecurityGroups)
			.WithB2CAuthority(ADConstants.AuthoritySignin)
			.WithRedirectUri($"msal{ADConstants.ClientId}://auth")
			.Build();
		}

		protected override void OnSleep () {
		}

		protected override void OnResume () {
			if (Cart.OrderItems == null)
				Cart.OrderItems = new List<OrderItem>();
		}
	}
}
