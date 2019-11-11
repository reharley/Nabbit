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
		private const string getQueueOrdersUrl = "http://localhost:7071/api/GetQueueOrders/restaurantId/{restaurantId}/allOrders/{allOrders}";
		//private const string getRestaurantsUrl = "https://nabbit.azurewebsites.net/api/userId/{userId}/schoolId/{schoolId}?code=ZX45t3u8uyrT24p6bbBFXhepqeQ7KoKGN9N/lbl1p8vakNTHsgw/ng==";
		private const string postRestaurantUrl = "http://localhost:7071/api/PostRestaurant";
		private const string postOrderUrl = "http://localhost:7071/api/PostOrder";
		//private const string getUserUrl = "http://localhost:7071/api/GetUser/userId/{userId}"; https://nabbit.azurewebsites.net/api/GetUser/userId/{userId}?code=Vziqr2EnpeTCyaxTQdPR49V3PMplIfhGrxjzfeZtdAwtld8sc5HtmA==
		private const string getUserUrl = "https://nabbit.azurewebsites.net/api/GetUser/userId/{userId}?code=Vziqr2EnpeTCyaxTQdPR49V3PMplIfhGrxjzfeZtdAwtld8sc5HtmA==";
		private const string getRestOrdersUrl = "http://localhost:7071/api/GetRestOrders/{restaurantId}";
		private const string postUserUrl = "http://localhost:7071/api/PostUser";
		private static string getSetupIntentUrl = "http://localhost:7071/api/GetSetupIntent";

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
			//var user = new User();
			//user.FirstName = "TJ";
			//GetUser().Wait();
			//var intent = GetSetupIntentAsync();
			//while (!intent.IsCompleted) {
			//	Thread.Sleep(10);
			//}
			//var result = intent.Result;
			//PostUser(user).Wait();

			//MakeOrder();
			//GetQueueOrders("681a6d33-beac-4928-8172-793c3e981bd5", true).Wait();

			//PullUserOrders().Wait();
			//PullRestOrders().Wait();
			decimal temp = 40m;
			Console.WriteLine(temp.ToString("00"));
			//SendNotification();
		}

		public static async Task GetQueueOrders (string restaurantId, bool allOrders = false) {
			var OrderQueue = new List<Order>();

			try {
				using (var client = new HttpClient()) {
					var url = getQueueOrdersUrl.Replace("{restaurantId}", restaurantId).Replace("{allOrders}", allOrders.ToString());
					string result = "";
					using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
						result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
						if (httpResponse.IsSuccessStatusCode) {
							OrderQueue = JsonConvert.DeserializeObject<List<Order>>(result)
								.OrderByDescending(x => x.PickupTime).ToList();
						}
					}
				}
			} catch (Exception ex) {
				/// TODO: Log error
				Console.WriteLine("Something is missing...", "The app was unable to load data. Please check the your connections and try again.");
				Console.WriteLine(ex.Message);
			}
		}

		public static async Task<Stripe.SetupIntent> GetSetupIntentAsync () {
			Stripe.SetupIntent setupIntent = null;
			using (var client = new HttpClient()) {
				var url = getSetupIntentUrl;
				using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
					var result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
					if (httpResponse.IsSuccessStatusCode) {
						setupIntent = JsonConvert.DeserializeObject<Stripe.SetupIntent>(result);
					}
				}
			}

			return setupIntent;
		}

		public static async Task GetUser () {
			using (var client = new HttpClient()) {
				var url = getUserUrl.Replace("{userId}", "5d2b6da2-3f67-4fd0-a3c8-678cbfb9d4f9");
				string result = "";

				using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
					result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

					// skip new users
					if (httpResponse.IsSuccessStatusCode) {
						var user = JsonConvert.DeserializeObject<User>(result);
						await PostUser(user);
					}
				}
			}
		}

		public static async Task PostUser (User user) {
			string url = postUserUrl;
			using (var client = new HttpClient()) {
				var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
				var result = await client.PostAsync(url, content);
			}
		}

		public static void MakeOrder() {
			var restaurantId = new Guid("681a6d33-beac-4928-8172-793c3e981bd5");
			var userId = new Guid("5d2b6da2-3f67-4fd0-a3c8-678cbfb9d4f9");
			List<Models.OrderItem> orderItems = new List<Models.OrderItem>() {
				new Models.OrderItem() {
					Quantity = 1,
					Instructions = "TEST*****",
				}
			};

			var json = "{\"OrderId\":\"9397c098-0a7c-429d-8775-e2e8149bc640\",\"RestaurantId\":\"681a6d33-beac-4928-8172-793c3e981bd5\",\"MenuId\":\"00000000-0000-0000-0000-000000000000\",\"UserId\":\"5d2b6da2-3f67-4fd0-a3c8-678cbfb9d4f9\",\"TransactionId\":\"00000000-0000-0000-0000-000000000000\",\"FirstName\":\"Robert\",\"LastName\":\"Harley\",\"CreationDate\":\"2019-10-29T11:05:57.067274-07:00\",\"PickupTime\":\"2019-10-29T11:10:57\",\"OrderStatus\":\"Created\",\"OrderTotal\":5.65470,\"ServiceCharge\":0.4,\"OrderSubtotal\":4.75,\"OrderTaxes\":0.50470,\"OrderItems\":[{\"OrderItemId\":\"82187a4b-950b-4550-b7de-5bb392eb24b4\",\"Instructions\":\"\",\"Quantity\":1,\"ItemPrice\":4.75,\"Product\":{\"ProductId\":\"6160ec0c-993c-4efa-a5b5-0e08a31b9709\",\"Name\":\"Double Burger\",\"Description\":null,\"Price\":4.75,\"AddonGroupIds\":null},\"Addons\":[{\"AddonId\":\"91675768-12b8-486b-8281-c7901dbc334b\",\"Name\":\"sm\",\"Price\":0.0},{\"AddonId\":\"d5c82f38-47fc-4789-8f32-134ce32c5519\",\"Name\":\"Bacon(4)\",\"Price\":0.0},{\"AddonId\":\"ba5fbe74-f083-42e3-94bb-d92ac2edc07e\",\"Name\":\"White\",\"Price\":0.0}]}],\"CreatedAt\":\"2019-10-29T11:05:57.066942-07:00\"}";
			var order = JsonConvert.DeserializeObject<Order>(json);
			order.OrderId = Guid.NewGuid();

			PostOrder(order).Wait();
		}

		static async Task PullRestOrders () {
			//var userId = "5d2b6da2-3f67-4fd0-a3c8-678cbfb9d4f9";
			var restaurantId = "681a6d33-beac-4928-8172-793c3e981bd5";
			try {
				using (var client = new HttpClient()) {
					var url = getRestOrdersUrl.Replace("{restaurantId}", restaurantId);
					var result = await client.GetStringAsync(url);
					if (result == "none")
						return;
					var orders = JsonConvert.DeserializeObject<List<Models.Order>>(result);
				}
			} catch (Exception ex) {
				/// TODO: Log error
				Console.WriteLine("Something is missing...", "The app was unable to load data. Please check the your connections and try again.");
				Console.WriteLine(ex.Message);
			}
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
					var orders = JsonConvert.DeserializeObject<List<Models.Order>>(result);
				}
			} catch (Exception ex) {
				/// TODO: Log error
				Console.WriteLine("Something is missing...", "The app was unable to load data. Please check the your connections and try again.");
				Console.WriteLine(ex.Message);
			}
		}

		public static async Task PostOrder(Models.Order order) {
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
						Rank = 1
						//OpeningHour = item[2],
						//ClosingHour = item[3]
					};

					restaurant.Menus.Add(menu);
				} else if (item[1] == "Open Hours") { // create the business hours
					//var businessHours = new BusinessHours() {
					//	DayOfWeek = Convert.ToByte(item[0]),
					//	OpeningHour = item[2],
					//	ClosingHour = item[3]
					//};

					//restaurant.BusinessHours.Add(businessHours);
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