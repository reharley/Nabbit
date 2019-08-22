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
using Microsoft.AppCenter.Auth;

namespace Nabbit {
	public partial class App : Application {
		public App() {
			InitializeComponent();
			
			MainPage = new AppShell();
		}

		protected override void OnStart() {
			// Handle when your app starts
			_ = LocalGlobals.PullObjects();
			Cart.OrderItems = new List<OrderItem>();


			AppCenter.Start("android=e0554b49-7cd0-4e03-992d-b7fe395fbc19;" + 
				"ios=2297da99-c2fd-4ead-abfb-930c804e2b79;",
				  typeof(Analytics), typeof(Crashes), typeof(Auth));
			
		}

		protected override void OnSleep() {
			// Handle when your app sleeps
		}

		protected override void OnResume() {
			// Handle when your app resumes
			if (Cart.OrderItems== null)
				Cart.OrderItems= new List<OrderItem>();
		}

		private void DisplayAlert(string v1, string v2) {
			throw new NotImplementedException();
		}
	}
}
