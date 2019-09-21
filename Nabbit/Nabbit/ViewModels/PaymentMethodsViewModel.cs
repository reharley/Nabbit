using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Nabbit.Models;
using Newtonsoft.Json.Linq;

namespace Nabbit.ViewModels {
	public class PaymentMethodsViewModel : BaseViewModel {
		List<PaymentMethod> paymentMethods;
		public List<PaymentMethod> PaymentMethods {
			get {
				if (paymentMethods == null)
					paymentMethods = new List<PaymentMethod>();

				return paymentMethods;
			}
			set {
				SetProperty(ref paymentMethods, value);
			}
		}

		public PaymentMethodsViewModel () {
		}

		public async Task GetPaymentMethods() {
			var baseAddress = new Uri("https://private-anon-124050f0da-fattmerchant.apiary-proxy.com/");

			using (var httpClient = new HttpClient { BaseAddress = baseAddress }) {

				var empty = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJtZXJjaGFudCI6IjA1ZDZkZDM4LTdlZjQtNDdlZi1iYTM1LWVlY2I1ZTE0MTkwMSIsImdvZFVzZXIiOmZhbHNlLCJzdWIiOiI0N2RkNTY5Yi1hZTVmLTQ2M2YtYmNiZC02NWUyOGIzMWRiNTYiLCJpc3MiOiJodHRwOi8vYXBpcHJvZC5mYXR0bGFicy5jb20vdGVhbS9hcGlrZXkiLCJpYXQiOjE1Njg3NTU1MDUsImV4cCI6NDcyMjM1NTUwNSwibmJmIjoxNTY4NzU1NTA1LCJqdGkiOiJyRXVyRFA2amdYUExKRk54In0.pm7ut5ywZMfuL23Cc0ZRyqAL_IBDh_DZmCxF7iAu_lQ";
				httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", $"Bearer {empty}");

				httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");

				using (var response = await httpClient.GetAsync("payment-method/")) {
					string responseData = await response.Content.ReadAsStringAsync();
					if (response.IsSuccessStatusCode) {
						var paymentMethodsObj = (JArray)JObject.Parse(responseData)["data"];


						for (int i = 0; i < paymentMethodsObj.Count; i++) {
							var cardType = paymentMethodsObj[i]["card_type"].ToString();
							cardType = cardType.Substring(0, 1).ToUpper() + cardType.Substring(1);

							var address1 = paymentMethodsObj[i]["address_1"].ToString();
							var address2 = paymentMethodsObj[i]["address_2"].ToString();
							var city = paymentMethodsObj[i]["address_city"].ToString();
							var state = paymentMethodsObj[i]["address_state"].ToString();
							var zip = paymentMethodsObj[i]["address_zip"].ToString();
							var billingAddress = $"{address1} {address2}\n"
												+$"{city}, {state}, {zip}";

							var payMethod = new PaymentMethod() {
								CardType = cardType,
								BillingAddress = billingAddress,
								CardExpire = paymentMethodsObj[i]["card_exp"].ToString().Insert(2, "/"),
								CardLastFour = paymentMethodsObj[i]["card_last_four"].ToString(),
								PersonName = paymentMethodsObj[i]["person_name"].ToString(),
								PaymentMethodId = Guid.Parse(paymentMethodsObj[i]["id"].ToString()),
							};

							PaymentMethods.Add(payMethod);
						}
					}
				}
			}
		}

	}
}
