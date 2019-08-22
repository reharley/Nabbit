using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Nabbit.Models;
using Nabbit.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nabbit.MenuUploader {
	class Program {
		static List<List<string>> items;
		static CloudTable restaurantTable;
		static CloudTable orderTable;

		static Restaurant restaurant;
		static RestaurantEntity restaurantEntity;
		static string accountKey = @"UuoGtBz5tkW0mJ2GLvLmquqB7eeQLFCN79RHAK9DqLdK1qVbgwW6GTM/M7IfSIuXFxQ7VpNoCBqNvMkJ/SQejQ==";
		static string storageName = @"nabbitbf38";
		private const string restaurantTableName = "restaurantTableDev";
		private const string orderTableName = "orderTableDev";
		private const string getSchoolsUrl = "http://localhost:7071/api/userId/{userId}";
		//private const string getSchoolsUrl = "https://nabbit.azurewebsites.net/api/userId/{userId}?code=ztgeYLZ/QNjKE26BoC9fb/R6PpvL0dNAlzH3r2dC0QUtwtaKs2tWsg==";
		private const string getRestaurantsUrl = "http://localhost:7071/api/userId/{userId}/schoolId/{schoolId}";
		//private const string getUserOrdersUrl = "http://localhost:7071/api/GetUserOrders/userId/{userId}";
		//private const string getUserOrdersUrl = "https://nabbit.azurewebsites.net/api/GetUserOrders/userId/{userId}?code=X3NJ2NZKahEziqSKZrlX/KxpoyWvuHfYE4wROOAjOLnNleMWGByFIA==";
		private const string getQueueOrdersUrl = "http://localhost:7071/api/GetQueueOrders/restaurantId/{restaurantId}";
		//private const string getRestaurantsUrl = "https://nabbit.azurewebsites.net/api/userId/{userId}/schoolId/{schoolId}?code=ZX45t3u8uyrT24p6bbBFXhepqeQ7KoKGN9N/lbl1p8vakNTHsgw/ng==";
		private const string postRestaurantUrl = "http://localhost:7071/api/PostRestaurant";
		private const string postOrderUrl = "http://localhost:7071/api/PostOrder";

		private static HttpClient _client;


		static CloudStorageAccount storageAccount;
		static CloudTableClient tableClient;

		private static HttpClient Client {
			get {
				if (_client == null) {
					_client = new HttpClient();
				}

				return _client;
			}
		}

		static void Main(string[] args) {
			//string menuFilename = @"EvCCMenu.csv";
			//ParseMenu(menuFilename);
			//PushToDB();
			//PushToTable();
			//CreateRestaurantTableAsync().Wait();
			//AzureFunctionTest().Wait();


			//PullObjects().Wait();
			//UpdateRestaurant().Wait();
			//PushToTable().Wait();

			MakeOrder();
			//PullUserOrders().Wait();

			Console.WriteLine("Complete");
			//SendNotification();
		}

		public static void MakeOrder() {
			var restaurantId = new Guid("681a6d33-beac-4928-8172-793c3e981bd5");
			var userId = new Guid("5d2b6da2-3f67-4fd0-a3c8-678cbfb9d4f9");
			List<OrderItem> orderItems = new List<OrderItem>() {
				new OrderItem() {
					Quantity = 1,
					Instructions = "TEST*****",
				}
			};

			var order = new Order(userId, restaurantId) {
				FirstName = "Emma",
				LastName = "Harley",
				PickupTime = DateTime.Now,
				OrderItems = orderItems,
				OrderTotal = 5m
			};

			PostOrder(order).Wait();
		}

		static async Task PullUserOrders() {
			//var userId = "5d2b6da2-3f67-4fd0-a3c8-678cbfb9d4f9";
			var restaurantId = "681a6d33-beac-4928-8172-793c3e981bd5";
			try {
				using (var client = new HttpClient()) {
					var url = getQueueOrdersUrl.Replace("{restaurantId}", restaurantId);
					var result = await client.GetStringAsync(url);
					if (result == "none")
						return;
					var orders = JsonConvert.DeserializeObject<List<Order>>(result);
				}
			} catch (Exception ex) {
				/// TODO: Log error
				Console.WriteLine("Something is missing...", "The app was unable to load data. Please check the your connections and try again.");
				Console.WriteLine(ex.Message);
			}
		}

		public static async Task PostOrder(Order order) {
			string url = postOrderUrl;
			using (var client = new HttpClient()) {
				var content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
				var result = await client.PostAsync(url, content);
			}
		}

		public static async Task UpdateRestaurant() {
			string url = postRestaurantUrl;
			restaurant.Version++;
			restaurantEntity = new RestaurantEntity(restaurant.RestaurantId.ToString(),
						JsonConvert.SerializeObject(restaurant));
			using (var client = new HttpClient()) {
				var content = new StringContent(restaurantEntity.JSON, Encoding.UTF8, "application/json");
				var result = await client.PostAsync(url, content);
			}
		}

		public static async Task<int> PullObjects() {
			try {
				using (var client = new HttpClient()) {
					var url = getSchoolsUrl.Replace("{userId}", "none");
					var result = await client.GetStringAsync(url);
					var schoolEntity = JsonConvert.DeserializeObject<List<SchoolEntity>>(result)[0];
					var school = JsonConvert.DeserializeObject<School>(schoolEntity.JSON);

					url = getRestaurantsUrl.Replace("{userId}", "none").Replace("{schoolId}", school.SchoolId.ToString());
					result = await client.GetStringAsync(url);
					var restaurantEntity = JsonConvert.DeserializeObject<List<RestaurantEntity>>(result)[0];
					restaurant = JsonConvert.DeserializeObject<Restaurant>(restaurantEntity.JSON);
				}
			} catch (Exception ex) {
				/// TODO: Log error
				Console.WriteLine("Something is missing...", "The app was unable to load data. Please check the your connections and try again.");
				Console.WriteLine(ex.Message);
				return -1;
			}
			return 0;
		}

		/// <summary>
		/// Sends the push notification to the restaurant device via OneSignal
		/// </summary>
		static void SendNotification() {
			var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

			request.KeepAlive = true;
			request.Method = "POST";
			request.ContentType = "application/json; charset=utf-8";

			byte[] byteArray = Encoding.UTF8.GetBytes("{"
													+ "\"app_id\": \"fe020159-a228-44d2-96af-8dc46096a219\","
													+ "\"headings\": {\"en\": \"ORDER UP\"},"
													+ "\"contents\": {\"en\": \"There's an order.\"},"
													+ "\"include_player_ids\": [\"f3ea84de-9287-4fd5-941d-bc585d59a763\"]}");

			string responseContent = null;

			try {
				using (var writer = request.GetRequestStream()) {
					writer.Write(byteArray, 0, byteArray.Length);
				}

				using (var response = request.GetResponse() as HttpWebResponse) {
					using (var reader = new StreamReader(response.GetResponseStream())) {
						responseContent = reader.ReadToEnd();
					}
				}
			} catch (WebException ex) {
				System.Diagnostics.Debug.WriteLine(ex.Message);
				System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
			}
		}
		static async Task AzureFunctionTest() {
			Exception error = null;

			try {
				var url = getSchoolsUrl.Replace("{userId}", "none");
				var result = await Client.GetStringAsync(url);
				Console.WriteLine("SchoolEntity: ");
				Console.WriteLine(result);
				var schoolEntity = JsonConvert.DeserializeObject<List<SchoolEntity>>(result)[0];
				var school = JsonConvert.DeserializeObject<School>(schoolEntity.JSON);
				Console.WriteLine("School: ");
				Console.WriteLine(schoolEntity.JSON);
				url = getRestaurantsUrl.Replace("{userId}", "none").Replace("{schoolId}", school.SchoolId.ToString());
				result = await Client.GetStringAsync(url);
				Console.WriteLine("RestEntity: ");
				Console.WriteLine(result);
				var restaurant = JsonConvert.DeserializeObject<List<RestaurantEntity>>(result)[0];
				Console.WriteLine("Rest: ");
				Console.WriteLine(restaurant.JSON);
			} catch (Exception ex) {
				error = ex;
			}

			if (error != null) {
				Console.WriteLine("Something went wrong.");
			}
		}

		static async Task PushToTable() {
			CloudStorageAccount storageAccount = new CloudStorageAccount(
				new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
					storageName, accountKey), true);
			CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
			restaurantTable = tableClient.GetTableReference(restaurantTableName);
			orderTable = tableClient.GetTableReference(orderTableName);
			await orderTable.CreateIfNotExistsAsync();
		}

		static async Task CreateRestaurantTableAsync() {
			// Create the CloudTable if it does not exist
			await restaurantTable.CreateIfNotExistsAsync();

			string restaurantJson = File.ReadAllText("restaurant.json");
			string schoolJson = File.ReadAllText("school.json");
			var restaurant = JsonConvert.DeserializeObject<Restaurant>(restaurantJson);
			var school = JsonConvert.DeserializeObject<School>(schoolJson);
			restaurantEntity = new RestaurantEntity(restaurant.RestaurantId.ToString(), restaurantJson);
			var schoolEntity = new SchoolEntity(school.SchoolId.ToString(), schoolJson);

			TableOperation insertRest = TableOperation.Insert(restaurantEntity);
			TableOperation insertSchool = TableOperation.Insert(schoolEntity);

			await restaurantTable.ExecuteAsync(insertRest);
			await restaurantTable.ExecuteAsync(insertSchool);

			Console.WriteLine("Done");
		}


		static void PushToDB() {
			School school = new School();
			Restaurant restaurant = null;

			foreach (var item in items) {
				if (item[2].Equals("Addon")) { // create the addons
					var addon = new Addon() {
						AddonId = Guid.NewGuid(),
						Name = item[0],
						Price = Convert.ToDecimal(item[1])
					};

					var addonGroup = restaurant.AddonGroups.Where(ag => ag.Name.Equals(item[3])).FirstOrDefault();
					if (addonGroup == null) {
						addonGroup = new AddonGroup() {
							AddonGroupId = Guid.NewGuid(),
							Name = item[3],
							SelectType = item[4]
						};

						restaurant.AddonGroups.Add(addonGroup);
					}

					addonGroup.AddonIds.Add(addon.AddonId);
					restaurant.Addons.Add(addon);
				} else if (item[1] == "School") { // create the school
					school = new School() {
						Name = item[0],
						ShortName = item[2],
						Location = "Everett",
						Logo = null
					};
				} else if (item[1] == "Restaurant") { // create the restaurant
													  // TODO: Check if restaurant name already exists
					if (restaurant != null)
						continue;

					restaurant = new Restaurant(school.SchoolId, item[0]);

					school.RestaurantIds.Add(restaurant.RestaurantId);
				} else if (item[1] == "Menu") { // create the menus
					if (restaurant.Menus
							.Where(a => a.Name.Equals(item[0]))
							.FirstOrDefault() != null)
						continue;

					var menu = new Menu() {
						MenuId = Guid.NewGuid(),
						Name = item[0],
						Description = "",
						Rank = 1,
						OpeningHour = item[2],
						ClosingHour = item[3]
					};

					restaurant.Menus.Add(menu);
				} else if (item[1] == "Open Hours") { // create the business hours
					var businessHours = new BusinessHours() {
						DayOfWeek = Convert.ToByte(item[0]),
						OpeningHour = item[2],
						ClosingHour = item[3]
					};

					restaurant.BusinessHours.Add(businessHours);
				} else { // create the products
					if (restaurant.Products.Where(a => a.Name.Equals(item[0])).FirstOrDefault() != null)
						continue;

					var product = new Product() {
						ProductId = Guid.NewGuid(),
						Name = item[0],
						Price = Convert.ToDecimal(item[1])
					};
					restaurant.Products.Add(product);

					var productCategory = restaurant.ProductCategories.Where(ag => ag.Name == item[3]).FirstOrDefault();
					// add product category if it doesn't exist yet
					if (productCategory == null) {
						productCategory = new ProductCategory() {
							ProductCategoryId = Guid.NewGuid(),
							Name = item[3],
							Rank = Convert.ToInt32(item[4]),
							Enabled = true
						};

						restaurant.ProductCategories.Add(productCategory);

						var menu = restaurant.Menus
							.Where(m => m.Name.Equals(item[2]))
							.FirstOrDefault();

						menu.ProductCategoryIds.Add(productCategory.ProductCategoryId);
					}

					productCategory.ProductIds.Add(product.ProductId);
				}
			}

			restaurant.Version = 1;
			string restaurantJson = JsonConvert.SerializeObject(restaurant);
			string schoolJson = JsonConvert.SerializeObject(school);

			System.IO.File.WriteAllText(@"restaurant.json", restaurantJson);
			System.IO.File.WriteAllText(@"school.json", schoolJson);
		}

		static void ParseMenu(string filename) {
			items = new List<List<string>>();
			int counter = 0;
			string line;

			// Read the file and display it line by line.  
			System.IO.StreamReader file =
				new System.IO.StreamReader(filename);
			while ((line = file.ReadLine()) != null) {
				if (counter > 0) {
					//System.Console.WriteLine(line);
					items.Add(new List<string>(line.Split(',')));
				}
				counter++;
			}

			file.Close();

			//foreach (var item in items) {
			//	Console.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}",
			//		item[0], item[1], item[2], item[3], item[4]));
			//}
		}
	}
}