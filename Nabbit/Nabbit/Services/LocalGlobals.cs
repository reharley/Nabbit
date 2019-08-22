using Nabbit.Models;
using Newtonsoft.Json;
using Plugin.SecureStorage;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Nabbit.Services {
	public static class LocalGlobals {
		private const string getSchoolsUrl = "https://nabbit.azurewebsites.net/api/userId/{userId}?code=ztgeYLZ/QNjKE26BoC9fb/R6PpvL0dNAlzH3r2dC0QUtwtaKs2tWsg==";
		private const string getRestaurantsUrl = "https://nabbit.azurewebsites.net/api/userId/{userId}/schoolId/{schoolId}?code=ZX45t3u8uyrT24p6bbBFXhepqeQ7KoKGN9N/lbl1p8vakNTHsgw/ng==";
		private const string postRestaurantUrl = "https://nabbit.azurewebsites.net/api/PostRestaurant?code=z/t3ObzhSTN1sHzTpHz8gS8rQdh7fATP9fO0sERiO334V3qNhdLl9A==";
		private const string postOrderUrl = "https://nabbit.azurewebsites.net/api/PostOrder?code=OZhGIGBClKKcGkqsIWNbyB0YG1ZqCoGyXVriNj5f9Ezbn0v1LwZeaQ==";
		private const string getUserOrdersUrl = "https://nabbit.azurewebsites.net/api/GetUserOrders/userId/{userId}?code=X3NJ2NZKahEziqSKZrlX/KxpoyWvuHfYE4wROOAjOLnNleMWGByFIA==";

		static Restaurant restaurant;
		public static Restaurant Restaurant {
			get {
				if (restaurant == null)
					PullObjects().Wait();

				return restaurant;
			}
			set {
				restaurant = value;
			}
		}
		static School school;
		public static School School {
			get {
				if (school == null) {
					var task = PullObjects();
					while (!task.IsCompleted)
						Thread.Sleep(3);
				}

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
				if (user == null || user.LoggedIn == false)
					GetUserInfo();

				return user;
			}
			set {
				user = value;
			}
		}

		public static void GetUserInfo() {
			user = new User();
			if (CrossSecureStorage.Current.HasKey("UserToken")) {
				var userIdToken = CrossSecureStorage.Current.GetValue("UserToken");
				var tokenHandler = new JwtSecurityTokenHandler();
				try {
					var jwToken = tokenHandler.ReadJwtToken(userIdToken);
					user.FirstName = jwToken.Claims.FirstOrDefault(t => t.Type == "given_name").Value;
					user.LastName = jwToken.Claims.FirstOrDefault(t => t.Type == "family_name").Value;
					user.UserId = new Guid(jwToken.Claims.FirstOrDefault(t => t.Type == "oid").Value);
					user.Email = jwToken.Claims.FirstOrDefault(t => t.Type == "emails").Value;

					user.LoggedIn = true;
				} catch (Exception e) {

				}
			}
		}

		/// <summary>
		/// Grabs school and restaurant objects from the azure storage table using azure functions.
		/// Once it is saved to the device, it will load from the device instead of pulling from the servers.
		/// </summary>
		/// <returns>0 on success; -1 on failure</returns>
		public static async Task<int> PullObjects() {
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

			/// TODO: Figure out an efficient way to check for version changes
			//bool schoolPresent = Application.Current.Properties.ContainsKey("school");
			//bool restaurantPresent = Application.Current.Properties.ContainsKey("restaurant");

			//if (!schoolPresent || !restaurantPresent) {

			//} else if (restaurant == null || School == null) {
			//	restaurant = JsonConvert.DeserializeObject<Restaurant>((string)App.Current.Properties["restaurant"]);
			//	school = JsonConvert.DeserializeObject<School>((string)App.Current.Properties["school"]);
			//}

			return 0;
		}

		public static async Task UpdateRestaurant(Restaurant rest) {
			string url = postRestaurantUrl;
			using (var client = new HttpClient()) {
				var content = new StringContent(JsonConvert.SerializeObject(rest), Encoding.UTF8, "application/json");
				var result = await client.PostAsync(url, content);
			}
		}

		public static async Task PostOrder(Order order) {
			string url = postOrderUrl;
			using (var client = new HttpClient()) {
				var content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
				var result = await client.PostAsync(url, content);
			}
		}

		public static void SaveRestaurant() {
			App.Current.Properties["restaurant"] = JsonConvert.SerializeObject(Restaurant);
		}

		public static async Task GetUserOrders() {
			try {
				using (var client = new HttpClient()) {
					string result = "";
					var url = getUserOrdersUrl.Replace("{userId}", user.UserId.ToString());
					using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
						result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
					}
					if (result == "none")
						return;
					userOrders = JsonConvert.DeserializeObject<List<Order>>(result);
				}
			} catch (Exception ex) {
				/// TODO: Log error
				Console.WriteLine("Something is missing...", "The app was unable to load data. Please check the your connections and try again.");
				Console.WriteLine(ex.Message);
			}
		}
	}
}
