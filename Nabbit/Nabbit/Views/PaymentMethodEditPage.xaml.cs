using System;
using System.Collections.Generic;
using System.Net.Http;
using Nabbit.Models;
using Nabbit.Services;
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
			using (var httpClient = new HttpClient { BaseAddress = baseAddress }) {
				httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", $"Bearer {LocalGlobals.empty}");

				httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
				using (var response = await httpClient.GetAsync($"payment-method/{paymentMethod.PaymentMethodId.ToString()}")) {
					string responseData = await response.Content.ReadAsStringAsync();
				}
			}
		}

		async void DeletePressed (object sender, EventArgs e) {

		}
	}
}
