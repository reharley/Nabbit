using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Nabbit.Models;
using Nabbit.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace Nabbit.Views {
	public partial class PaymentMethodEditPage : ContentPage {
		PaymentMethod paymentMethod;

		public PaymentMethodEditPage (PaymentMethod payMethod) {
			InitializeComponent();
			BindingContext = paymentMethod = payMethod;
		}

		async void SavePressed (object sender, EventArgs e) {
			var baseAddress = new Uri("https://private-anon-7203bc3b4c-fattmerchant.apiary-proxy.com/");
			var url = $"payment-method/{paymentMethod.PaymentMethodId.ToString()}";
			using (var httpClient = new HttpClient { BaseAddress = baseAddress }) {
				httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", $"Bearer {LocalGlobals.empty}");

				httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
				using (var response = await httpClient.GetAsync(url)) {
					string responseData = await response.Content.ReadAsStringAsync();


					var paymentMethodsObj = JObject.Parse(responseData);


					// make sure this doesn't have a slash
					var hey = paymentMethodsObj["card_exp"] = paymentMethod.CardExpire.Remove(2, 3);

					paymentMethodsObj["method"] = "card";
					paymentMethodsObj["bank_name"] = "n/a";
					paymentMethodsObj["bank_type"] = "checking";
					paymentMethodsObj["bank_holder_type"] = "personal";
					paymentMethodsObj["person_name"] = paymentMethod.PersonName;
					paymentMethodsObj["address_1"] = paymentMethod.Address1;
					paymentMethodsObj["address_2"] = paymentMethod.Address2;
					var city = paymentMethodsObj["address_city"] = paymentMethod.City;
					var state = paymentMethodsObj["address_state"] = paymentMethod.State;
					var zip = paymentMethodsObj["address_zip"] = paymentMethod.Zip;

					var wow = paymentMethodsObj.ToString(Formatting.None);
					var content = new StringContent(paymentMethodsObj.ToString(Formatting.None), Encoding.UTF8, "application/json");
					var result = await httpClient.PutAsync(url, content);
					var resultMessage = await result.Content.ReadAsStringAsync();
				}
			}

			await Navigation.PopAsync();
		}

		async void DeletePressed (object sender, EventArgs e) {
			var baseAddress = new Uri("https://apiprod.fattlabs.com/");
			var url = $"payment-method/{paymentMethod.PaymentMethodId.ToString()}";

			using (var httpClient = new HttpClient { BaseAddress = baseAddress }) {


				httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", $"Bearer {LocalGlobals.empty}");

				httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");

				using (var response = await httpClient.DeleteAsync(url)) {

					string responseData = await response.Content.ReadAsStringAsync();
				}
			}

			await Navigation.PopAsync();
		}
	}
}
