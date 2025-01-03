﻿using Nabbit.Models;
using Nabbit.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.SecureStorage;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Nabbit.Services {
	public static class LocalGlobals {
#if DEBUG
		private const string getSchoolsUrl = "https://nabbitdev.azurewebsites.net/api/userId/{userId}?code=0y3yfBDdRd4iZBAgUmQzuxZAma1uvtRUCZ3iAi9TvY7Pbp7CO35Rvw==";
		private const string getRestaurantsUrl = "https://nabbitdev.azurewebsites.net/api/userId/{userId}/schoolId/{schoolId}?code=IPJvBINpgLWBtZMltTWzp1gKhp7K3UYr8GDZH8XlSoRMgyiFhq9Okg==";
		private const string postRestaurantUrl = "https://nabbitdev.azurewebsites.net/api/PostRestaurant?code=ex3tlHiPFt8vM41DpscY3CM6hJTpUG8J8iXr7NGB8NOkRhTf9O/K/w==";
		private const string postOrderUrl = "https://nabbitdev.azurewebsites.net/api/PostOrder?code=Jtr7df6LsRbjAem1bNNOA0Ud2ShquAmH4/gKej/azcDIPazMfRPI1g==";
		private const string postUserUrl = "https://nabbitdev.azurewebsites.net/api/PostUser?code=stI4oljW/w4fJRBQas45SES2D54FuLWX7Rk4b6Wgl1nfBGGfvxuc4g==";
		private const string getUserUrl = "https://nabbitdev.azurewebsites.net/api/GetUser/userId/{userId}?code=dH3WJmZ9759Z9SCNP9zsrOZHwFhXlXn048AfRypgjswv17wNkRRJng==";
		private const string getUserOrdersUrl = "https://nabbitdev.azurewebsites.net/api/GetUserOrders/userId/{userId}?code=sBwwBqgdOcGrdtWUFDn13d71nnoz2pMz2124c99IUHqxalN749TkFg==";
		private const string pingRestaurantUrl = "https://nabbitdev.azurewebsites.net/api/PingRestaurant/restaurantId/{restaurantId}/deviceId/{deviceId}?code=3M2fgihbOMVXxORHAe9z0eMTmPOMuvdRjcFnnPiduvUM6dBhWGILAQ==";
		private const string getRestOrdersUrl = "https://nabbitdev.azurewebsites.net/api/GetRestOrders/{restaurantId}?code=uxhruPqapLoeiV65pNDmN8wEEmW0/ul4Z9Q0whqohHEcYxwWUSRO4w==";
#else
		private const string getSchoolsUrl = "https://nabbit.azurewebsites.net/api/userId/{userId}?code=ztgeYLZ/QNjKE26BoC9fb/R6PpvL0dNAlzH3r2dC0QUtwtaKs2tWsg==";
		private const string getRestaurantsUrl = "https://nabbit.azurewebsites.net/api/userId/{userId}/schoolId/{schoolId}?code=ZX45t3u8uyrT24p6bbBFXhepqeQ7KoKGN9N/lbl1p8vakNTHsgw/ng==";
		private const string postRestaurantUrl = "https://nabbit.azurewebsites.net/api/PostRestaurant?code=z/t3ObzhSTN1sHzTpHz8gS8rQdh7fATP9fO0sERiO334V3qNhdLl9A==";
		private const string postOrderUrl = "https://nabbit.azurewebsites.net/api/PostOrder?code=OZhGIGBClKKcGkqsIWNbyB0YG1ZqCoGyXVriNj5f9Ezbn0v1LwZeaQ==";
		private const string postUserUrl = "https://nabbit.azurewebsites.net/api/PostUser?code=CoamMBwbH3aqVfeVzzRJuXrw2n6FVHCsT0l26phUAwL0SsTBEwTPyw==";
		private const string getUserUrl = "https://nabbit.azurewebsites.net/api/GetUser/userId/{userId}?code=Vziqr2EnpeTCyaxTQdPR49V3PMplIfhGrxjzfeZtdAwtld8sc5HtmA==";
		private const string getUserOrdersUrl = "https://nabbit.azurewebsites.net/api/GetUserOrders/userId/{userId}?code=X3NJ2NZKahEziqSKZrlX/KxpoyWvuHfYE4wROOAjOLnNleMWGByFIA==";
		private const string pingRestaurantUrl = "https://nabbit.azurewebsites.net/api/PingRestaurant/restaurantId/{restaurantId}/deviceId/{deviceId}?code=YaiWFyQL3DE8DsLslelW/onWmSvdEAyVZFjQYH5ECHgHHlT71RMD9w==";
		private const string getRestOrdersUrl = "https://nabbit.azurewebsites.net/api/GetRestOrders/{restaurantId}?code=ZCJqEFJhas1iI2fsO0yQq24TAXRULXzx8ebr/s8Dr43MOSYGxNlZnA==";
#endif
		public const decimal TaxRate = 0.098m;

		public static int PingMinuteDelay = 1;

		static Restaurant restaurant;
		public static Restaurant Restaurant {
			get {
				return restaurant;
			}
			set {
				restaurant = value;
			}
		}

		static School school;
		public static School School {
			get {

				return school;
			}
			set {
				school = value;
			}
		}

		static List<Order> userOrders;
		public static List<Order> UserOrders {
			get {
				if (userOrders == null) {
					var task = GetUserOrders();
					while (!task.IsCompleted)
						Thread.Sleep(3);
				}
				return userOrders;
			}
			set {
				userOrders = value;
			}
		}

		static User user;
		public static User User {
			get {
				if (user == null)
					GetUserInfo();
				return user;
			}
			set {
				user = value;
			}
		}

		static void GetUserInfo () {
			user = new User();

			if (CrossSecureStorage.Current.HasKey("User")) {
				var userJson = CrossSecureStorage.Current.GetValue("User");
				user = JsonConvert.DeserializeObject<User>(userJson);
				user.LoggedIn = true;
			}
		}

		public static async Task GetUser (bool firstTry = true) {
			using (var client = new HttpClient()) {
				var url = getUserUrl.Replace("{userId}", user.UserId.ToString());
				string result = "";

				using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
					result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);


					// skip new users
					if (httpResponse.IsSuccessStatusCode) {
						user = JsonConvert.DeserializeObject<User>(result);
						user.LoggedIn = true;
					}
				}
			}

			if (user.SchoolId == Guid.Empty) {
				user.SchoolId = LocalGlobals.School.SchoolId;
				await SaveUser();
			}

			if (user.CustomerId == null || user.CustomerId == "") {
				var t = await StripeService.CreateCustomer(user.UserId.ToString());
				if (t.success) {
					user.CustomerId = t.customerId;
					await SaveUser();
				}
			}

			if (!CrossSecureStorage.Current.HasKey("User"))
				CrossSecureStorage.Current.SetValue("User", JsonConvert.SerializeObject(user));

			user.LoggedIn = true;
		}

		public static async Task SaveUser () {
			if (user == null)
				return;

			CrossSecureStorage.Current.SetValue("User", JsonConvert.SerializeObject(user));
			await PostUser();
		}

		/// <summary>
		/// Grabs school and restaurant objects from the azure storage table using azure functions.
		/// Once it is saved to the device, it will load from the device instead of pulling from the servers.
		/// </summary>
		/// <returns>0 on success; -1 on failure</returns>
		public static async Task<int> PullObjects (bool forcePull = false) {
			bool pull = false;
			bool schoolPresent = Application.Current.Properties.ContainsKey("school");
			bool restaurantPresent = Application.Current.Properties.ContainsKey("restaurant");

			if (forcePull || !schoolPresent || !restaurantPresent)
				pull = true;

			if (pull) {
				try {
					using (var client = new HttpClient()) {
						var url = getSchoolsUrl.Replace("{userId}", "none");
						string result = "";

						using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
							result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
						}
						var schoolEntity = JsonConvert.DeserializeObject<List<SchoolEntity>>(result)[0];
						school = JsonConvert.DeserializeObject<School>(schoolEntity.JSON);
						App.Current.Properties["school"] = schoolEntity.JSON;

						url = getRestaurantsUrl.Replace("{userId}", "none").Replace("{schoolId}", School.SchoolId.ToString());
						using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
							result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
						}
						var restaurantEntity = JsonConvert.DeserializeObject<List<RestaurantEntity>>(result)[0];
						restaurant = JsonConvert.DeserializeObject<Restaurant>(restaurantEntity.JSON);
						App.Current.Properties["restaurant"] = restaurantEntity.JSON;

						await App.Current.SavePropertiesAsync();
					}
				} catch (Exception ex) {
					/// TODO: Log error
					return -1;
				}
			} else if (restaurant == null || school == null) {
				restaurant = JsonConvert.DeserializeObject<Restaurant>((string)App.Current.Properties["restaurant"]);
				school = JsonConvert.DeserializeObject<School>((string)App.Current.Properties["school"]);
			}

			return 0;
		}

		public static async Task GetRestaurant () {
			using (var client = new HttpClient()) {
				var url = getRestaurantsUrl.Replace("{userId}", "none").Replace("{schoolId}", School.SchoolId.ToString());
				using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
					var result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
					if (httpResponse.IsSuccessStatusCode) {
						var restaurantEntity = JsonConvert.DeserializeObject<List<RestaurantEntity>>(result)[0];
						restaurant = JsonConvert.DeserializeObject<Restaurant>(restaurantEntity.JSON);
					}
				}
			}
		}

		public static async Task<(PingRestaurantResponse, bool)> PingRestaurant (string restaurantId, string deviceId) {
			using (var client = new HttpClient()) {
				var url = pingRestaurantUrl
					.Replace("{restaurantId}", restaurantId)
					.Replace("{deviceId}", deviceId);
				using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
					var result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
					if (httpResponse.IsSuccessStatusCode) {
						var pingRestaurantResponse = JsonConvert.DeserializeObject<PingRestaurantResponse>(result);
						return (pingRestaurantResponse, httpResponse.IsSuccessStatusCode);
					}

				}
			}
					return (new PingRestaurantResponse(), false);
		}

		public static async Task<bool> UpdateRestaurant (Restaurant rest) {
			string url = postRestaurantUrl;
			using (var client = new HttpClient()) {
				var content = new StringContent(JsonConvert.SerializeObject(rest), Encoding.UTF8, "application/json");
				var result = await client.PostAsync(url, content);
				return result.IsSuccessStatusCode;
			}
		}

		public static async Task PostOrder (Order order) {
			string url = postOrderUrl;
			using (var client = new HttpClient()) {
				var content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
				var result = await client.PostAsync(url, content);
			}
		}

		public static async Task PostUser () {
			string url = postUserUrl;
			using (var client = new HttpClient()) {
				var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
				var result = await client.PostAsync(url, content);
			}
		}

		public static async Task SaveRestaurant () {
			Restaurant.Version++;
			await UpdateRestaurant(Restaurant);
			App.Current.Properties["restaurant"] = JsonConvert.SerializeObject(Restaurant);
		}

		public static async Task<List<Order>> GetRestOrders () {
			try {
				using (var client = new HttpClient()) {
					string result = "";
					var url = getRestOrdersUrl.Replace("{restaurantId}", Restaurant.RestaurantId.ToString());
					using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
						result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

						if (httpResponse.IsSuccessStatusCode)
							return JsonConvert.DeserializeObject<List<Order>>(result);
						else
							return null;
					}
				}
			} catch (Exception ex) {
				/// TODO: Log error
				Console.WriteLine("Something is missing...", "The app was unable to load data. Please check the your connections and try again.");
				Console.WriteLine(ex.Message);

				return null;
			}
		}

		public static async Task GetUserOrders () {
			try {
				using (var client = new HttpClient()) {
					string result = "";
					var url = getUserOrdersUrl.Replace("{userId}", user.UserId.ToString());
					using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
						result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

						if (httpResponse.IsSuccessStatusCode)
							userOrders = JsonConvert.DeserializeObject<List<Order>>(result);
					}
				}
			} catch (Exception ex) {
				/// TODO: Log error
				Console.WriteLine("Something is missing...", "The app was unable to load data. Please check the your connections and try again.");
				Console.WriteLine(ex.Message);
			}
		}

		public static void Logout () {
			if (CrossSecureStorage.Current.HasKey("User"))
				CrossSecureStorage.Current.DeleteKey("User");

			user = new User();
		}
	}
}
