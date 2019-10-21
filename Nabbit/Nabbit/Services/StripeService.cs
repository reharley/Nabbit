using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Stripe;

namespace Nabbit.Services {
	public static class StripeService {
		private static string getSetupIntentUrl = "https://nabbit.azurewebsites.net/api/GetSetupIntent?code=BLdO/jwQS6KaHihe1ZaVnkDhtbaUvckE6iflqNRZ9oRJgFltDdZKng==";
		private static string getCustomerUrl = "https://nabbit.azurewebsites.net/api/GetCustomer/custId/{custId}?code=Izs5DTh9uPnD6bCipXVFWnraQ5W9ypyQFBQLQ9mXtbo6sVGDyQjf8g==";
		private static string getPayMethodsUrl = "https://nabbit.azurewebsites.net/api/GetPayMethods/customerId/{customerId}?code=3MX6pWjMZu1iu1kNaLUVQmUOQfMSz64QBgwiyD7cU/BtBHEaQN83Yw==";
		private static string getPayIntentUrl = "https://nabbit.azurewebsites.net/api/GetPayIntent/amount/{amount}/customerId/{customerId}/paymentMethodId/{paymentMethodId}?code=YfPY4BuJFeyECvLJXJfOd/bkWz/car8SmWlKeUBu3y0Pnl7DqvMyGA==";
		private static string attachUserPaymentUrl = "https://nabbit.azurewebsites.net/api/AttachUserPayment/custId/{custId}/payId/{payId}?code=atm9rtlRkGB63oaqZakHrMDRrEJjpaJO4wYGaye/GPIRkx4kfcMRZQ==";
		private static string pubKey = "pk_test_zFFDBaQm00tzDkEh04fd3vOS000CPjQesc";

		public static async Task<SetupIntent> GetSetupIntentAsync () {
			SetupIntent setupIntent = null;
			using (var client = new HttpClient()) {
				var url = getSetupIntentUrl;
				using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
					var result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
					if (httpResponse.IsSuccessStatusCode) {
						setupIntent = JsonConvert.DeserializeObject<SetupIntent>(result);
					}
				}
			}

			return setupIntent;
		}

		public static async Task<bool> AttachUserPayment (string customerId, string paymentMethodId) {
			using (var client = new HttpClient()) {
				var url = attachUserPaymentUrl.Replace("{custId}", customerId).Replace("{payId}", paymentMethodId);
				using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
					var result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
					return httpResponse.IsSuccessStatusCode;
				}
			}
		}

		public static async Task<string> ChargeAsync (string amount, string customerId, string paymentMethodId) {
			using (var client = new HttpClient()) {
				var url = getPayIntentUrl
					.Replace("{amount}", amount)
					.Replace("{customerId}", customerId)
					.Replace("{paymentMethodId}", paymentMethodId);

				using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
					var result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
					if (httpResponse.IsSuccessStatusCode) {
						var paymentIntent = JsonConvert.DeserializeObject<PaymentIntent>(result);

						if (paymentIntent.Id == null || paymentIntent.Id == "")
							return "failed";

						StripeConfiguration.ApiKey = pubKey;
						var service = new PaymentIntentService();
						var paymentIntentOptions = new PaymentIntentConfirmOptions() {
							ClientSecret = paymentIntent.ClientSecret
						};

						var intent = await service.ConfirmAsync(paymentIntent.Id, paymentIntentOptions);
						return intent.Status;
					}
				}
			}

			return "failed";
		}

		public static async Task<List<Models.PaymentMethod>> GetPayMethodsAsync (string customerId) {
			StripeList<PaymentMethod> stripePayMethods = null;
			using (var client = new HttpClient()) {
				var url = getPayMethodsUrl.Replace("{customerId}", customerId);
				using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
					var result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
					if (httpResponse.IsSuccessStatusCode) {
						stripePayMethods = JsonConvert.DeserializeObject<StripeList<PaymentMethod>>(result);
					}
				}
			}

			if (stripePayMethods == null)
				return null;

			var payMethods = new List<Models.PaymentMethod>();
			foreach (var payMethod in stripePayMethods) {
				var newPayMethod = new Models.PaymentMethod() {
					PersonName = payMethod.BillingDetails.Name,
					Address1 = payMethod.BillingDetails.Address.Line1,
					Address2 = payMethod.BillingDetails.Address.Line2,
					Zip = payMethod.BillingDetails.Address.PostalCode,
					State = payMethod.BillingDetails.Address.State,
					Country = payMethod.BillingDetails.Address.Country,
					City = payMethod.BillingDetails.Address.City,
					CardLastFour = payMethod.Card.Last4,
					MonthExpire = payMethod.Card.ExpMonth.ToString("00"),
					YearExpire = payMethod.Card.ExpYear.ToString(),
					PaymentMethodId = payMethod.Id,
					CardType = payMethod.Card.Brand.ToUpper()
				};

				newPayMethod.BuildBillingAddress();
				payMethods.Add(newPayMethod);
			}

			return payMethods;
		}
	}
}
