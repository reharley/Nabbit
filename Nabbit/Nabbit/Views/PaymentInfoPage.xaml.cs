using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Nabbit.Services;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace Nabbit.Views {
	public partial class PaymentInfoPage : ContentPage {
		public PaymentInfoPage () {
			InitializeComponent();
			webView.Source = DependencyService.Get<IBaseUrl>().Get() + "/payment.html";
            var tmp = webView.EvaluateJavaScriptAsync("assignValues(4,5)");
		}

		async void WebViewNavigating(object sender, WebNavigatingEventArgs e) {
			if (e.Url.Contains("payment_method")) {
				var paymentMethodId = e.Url.Split('?')[1];
				LocalGlobals.User.PaymentMethodId = Guid.Parse(paymentMethodId);
				e.Cancel = true;
				if (LocalGlobals.User.CustomerId == Guid.Empty)
					await CreateCustomer();

				await Navigation.PopAsync();
			}
		}

		async Task CreateCustomer () {
			var baseAddress = new Uri("https://apiprod.fattlabs.com/");

			using (var httpClient = new HttpClient { BaseAddress = baseAddress }) {
				var empty = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJtZXJjaGFudCI6IjA1ZDZkZDM4LTdlZjQtNDdlZi1iYTM1LWVlY2I1ZTE0MTkwMSIsImdvZFVzZXIiOmZhbHNlLCJzdWIiOiI0N2RkNTY5Yi1hZTVmLTQ2M2YtYmNiZC02NWUyOGIzMWRiNTYiLCJpc3MiOiJodHRwOi8vYXBpcHJvZC5mYXR0bGFicy5jb20vdGVhbS9hcGlrZXkiLCJpYXQiOjE1Njg3NTU1MDUsImV4cCI6NDcyMjM1NTUwNSwibmJmIjoxNTY4NzU1NTA1LCJqdGkiOiJyRXVyRFA2amdYUExKRk54In0.pm7ut5ywZMfuL23Cc0ZRyqAL_IBDh_DZmCxF7iAu_lQ";
				httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", $"Bearer {empty}");

				httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");

				var user = LocalGlobals.User;
				using (var content = new StringContent("{  \"firstname\": \"" + user.FirstName + "\",  \"lastname\": \"" + user.LastName + "\"}",
															System.Text.Encoding.Default, "application/json")) {
					using (var response = await httpClient.PostAsync("customer", content)) {
						string responseData = await response.Content.ReadAsStringAsync();
						var responseObj = JObject.Parse(responseData);
						if (responseObj["id"] != null)
							LocalGlobals.User.CustomerId = Guid.Parse((string)(responseObj["id"]));
					}
				}
			}
		}
	}
}
