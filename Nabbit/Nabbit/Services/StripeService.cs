using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Stripe;

namespace Nabbit.Services {
	public static class StripeService {
#if DEBUG
		private static string getSetupIntentUrl = "https://nabbitdev.azurewebsites.net/api/GetSetupIntent?code=gaIBcw8vXtVJpDZVwlsPq9ZYrYNfRhznNC0Y32thvXCd2JRf31Tztg==";
		private static string getPayMethodsUrl = "https://nabbitdev.azurewebsites.net/api/GetPayMethods/customerId/{customerId}?code=Pp55GvJeqXmYe6CHumcqVCaTSnQtwMxANZchOUTrfq9dZLeH0KaArg==";
		private static string getPayIntentUrl = "https://nabbitdev.azurewebsites.net/api/GetPayIntent/amount/{amount}/customerId/{customerId}/paymentMethodId/{paymentMethodId}?code=Y9N6KaCl8TztqghstJysL82zraCFHVddvHcbQp4GU8DBegEGn9GN5w==";
		private static string getPubKeyUrl = "https://nabbitdev.azurewebsites.net/api/GetPubKey?code=Raxl0hKzCEMQnWyKiBT0NKUhaxtHU0IFL72nXugzm9BmfBwbgx2fcw==";
		private static string attachUserPaymentUrl = "https://nabbitdev.azurewebsites.net/api/AttachUserPayment/custId/{custId}/payId/{payId}?code=KyjQKC7X/tv2JPolWqK4CaT54KjWF9voVIgLIurdafmZoyZgjRlnEw==";
		private static string detachUserPaymentUrl = "https://nabbitdev.azurewebsites.net/api/DetachPayMethod/paymentMethodId/{paymentMethodId}?code=269Ts6EiL4gesUSJsvwwlo7EYOMNuj0irnP08EeGWluZKvkaK17k9w==";
#else
		private static string getSetupIntentUrl = "https://nabbit.azurewebsites.net/api/GetSetupIntent?code=BLdO/jwQS6KaHihe1ZaVnkDhtbaUvckE6iflqNRZ9oRJgFltDdZKng==";
		private static string getCustomerUrl = "https://nabbit.azurewebsites.net/api/GetCustomer/custId/{custId}?code=Izs5DTh9uPnD6bCipXVFWnraQ5W9ypyQFBQLQ9mXtbo6sVGDyQjf8g==";
		private static string getPayMethodsUrl = "https://nabbit.azurewebsites.net/api/GetPayMethods/customerId/{customerId}?code=3MX6pWjMZu1iu1kNaLUVQmUOQfMSz64QBgwiyD7cU/BtBHEaQN83Yw==";
		private static string getPayIntentUrl = "https://nabbit.azurewebsites.net/api/GetPayIntent/amount/{amount}/customerId/{customerId}/paymentMethodId/{paymentMethodId}?code=YfPY4BuJFeyECvLJXJfOd/bkWz/car8SmWlKeUBu3y0Pnl7DqvMyGA==";
		private static string getPubKeyUrl = "https://nabbit.azurewebsites.net/api/GetPubKey?code=mJUsrR07LfUH3haW4Lfu2SZaVHJLqtsFnFeubl9bxcpSNX4r9Ddu0Q==";
		private static string attachUserPaymentUrl = "https://nabbit.azurewebsites.net/api/AttachUserPayment/custId/{custId}/payId/{payId}?code=atm9rtlRkGB63oaqZakHrMDRrEJjpaJO4wYGaye/GPIRkx4kfcMRZQ==";
		private static string detachUserPaymentUrl = "https://nabbit.azurewebsites.net/api/DetachPayMethod/paymentMethodId/{paymentMethodId}?code=2UGRo9G5mYRsOR2TEgWdktQAl9LgrAKIUJiF7NfKmZ93joPTWunJYQ==";
#endif
		private static string pubKey = "";

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

		public static async Task<(bool success, string customerId)> CreateCustomer (string userId) {
			using (var client = new HttpClient()) {
				var url = attachUserPaymentUrl.Replace("{userId}", userId);
				using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
					var result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
					var customerId = result;
					return (httpResponse.IsSuccessStatusCode, customerId);
				}
			}
		}

		public static async Task<bool> GetPubKey () {
			using (var client = new HttpClient()) {
				var url = getPubKeyUrl;
				using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
					pubKey = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
					if (!httpResponse.IsSuccessStatusCode)
						pubKey = "";
					return httpResponse.IsSuccessStatusCode;
				}
			}
		}

		public static async Task DetachUserPayment (string paymentMethodId) {
			using (var client = new HttpClient()) {
				var url = detachUserPaymentUrl.Replace("{paymentMethodId}", paymentMethodId);
				using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
					var result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
				}
			}
		}

		public static async Task<string> ChargeAsync (string amount, string customerId, string paymentMethodId) {
			if (pubKey == "") {
				if (await GetPubKey() == false)
					return "failed";
			}

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

						PaymentIntent intent;
						try {
							intent = await service.ConfirmAsync(paymentIntent.Id, paymentIntentOptions);
							return intent.Status;
						} catch (Exception e) {
							return "failed";
						}
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

		public static async Task<bool> AttachPayMethod (Models.PaymentMethod payMethod) {
			if (pubKey == "") {
				if (await GetPubKey() == false)
					return false;
			}

			StripeConfiguration.ApiKey = pubKey;
			var setupIntent = await StripeService.GetSetupIntentAsync();

			var line1 = payMethod.Address1;
			var line2 = payMethod.Address2;
			var city = payMethod.City;
			var state = payMethod.State;
			var zip = payMethod.Zip;
			var country = payMethod.Country;
			if (country != null)
				country = country.ToLower();

			var billing = new BillingDetailsOptions() {
				Name = payMethod.PersonName,
				Address = new AddressOptions() {
					Line1 = line1,
					Line2 = line2,
					City = city,
					State = state,
					PostalCode = zip,
					Country = country
				}
			};

			var payOptions = new PaymentMethodCreateOptions {
				Type = "card",
				Card = new PaymentMethodCardCreateOptions {
					Number = payMethod.CardNumber,
					ExpMonth = long.Parse(payMethod.MonthExpire),
					ExpYear = long.Parse(payMethod.YearExpire),
					Cvc = payMethod.CVV
				},
				BillingDetails = billing
			};

			var payService = new PaymentMethodService();
			PaymentMethod paymentMethod;
			try {
				paymentMethod = await payService.CreateAsync(payOptions);
			} catch (Exception e) {
				return false;
			}

			var confirmOptions = new SetupIntentConfirmOptions() {
				ClientSecret = setupIntent.ClientSecret,
				PaymentMethodId = paymentMethod.Id
			};

			var setupService = new SetupIntentService();
			try {
				var completeSetupIntent = await setupService.ConfirmAsync(setupIntent.Id, confirmOptions);
				if (completeSetupIntent.Status != "succeeded")
					return false;
			} catch (Exception exc) {
				return false;
			}

			if (LocalGlobals.User.CustomerId == null || LocalGlobals.User.CustomerId == "")
				await LocalGlobals.GetUser();

			bool success = await StripeService.AttachUserPayment(LocalGlobals.User.CustomerId, paymentMethod.Id);
			return success;
		}
	}
}
