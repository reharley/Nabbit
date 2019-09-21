using Microsoft.AppCenter.Auth;
using Nabbit.Services;
using Nabbit.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.SecureStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nabbit.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AccountPage : ContentPage {
		AccountViewModel viewModel;
		public AccountPage () {
			InitializeComponent();
			test.Text = "Click to sign in";

			LocalGlobals.GetUser().Wait();
			BindingContext = viewModel = new AccountViewModel();
			test.Text = "Logged In";
		}


		protected override void OnAppearing () {
			viewModel.UserInfo = LocalGlobals.User;
			firstName.Text = "Welcome, " + viewModel.UserInfo.FirstName;
		}

		private async void OrderHistoryClicked (object sender, EventArgs e) {
			await Shell.Current.GoToAsync("orderHistory");
		}

		private async void PaymentMethodsClicked (object sender, EventArgs e) {
			await Shell.Current.GoToAsync("paymentMethods");
		}

		private async void TestChargeClicked (object sender, EventArgs e) {
			var baseAddress = new Uri("https://apiprod.fattlabs.com/");
			using (var httpClient = new HttpClient { BaseAddress = baseAddress }) {
				var empty = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJtZXJjaGFudCI6IjA1ZDZkZDM4LTdlZjQtNDdlZi1iYTM1LWVlY2I1ZTE0MTkwMSIsImdvZFVzZXIiOmZhbHNlLCJzdWIiOiI0N2RkNTY5Yi1hZTVmLTQ2M2YtYmNiZC02NWUyOGIzMWRiNTYiLCJpc3MiOiJodHRwOi8vYXBpcHJvZC5mYXR0bGFicy5jb20vdGVhbS9hcGlrZXkiLCJpYXQiOjE1Njg3NTU1MDUsImV4cCI6NDcyMjM1NTUwNSwibmJmIjoxNTY4NzU1NTA1LCJqdGkiOiJyRXVyRFA2amdYUExKRk54In0.pm7ut5ywZMfuL23Cc0ZRyqAL_IBDh_DZmCxF7iAu_lQ";
				httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", $"Bearer {empty}");

				httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");

				var obj = "{" +
						$"\"payment_method_id\": \"{LocalGlobals.User.PaymentMethodIds[0].ToString()}\"," +
						"\"meta\": {\"tax\":2,\"subtotal\":10}," + 
						"\"total\": 12.00," +
						"\"pre_auth\": 0" +
					"}";
				using (var content = new StringContent(obj, System.Text.Encoding.Default, "application/json")) {
					using (var response = await httpClient.PostAsync("charge", content)) {
						string responseData = await response.Content.ReadAsStringAsync();
					}
				}
			}
		}
	}
}