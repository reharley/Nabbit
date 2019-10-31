using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Nabbit.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace Nabbit.Functions {
	public static class PostOrder {
		static CloudStorageAccount storageAccount;
		static CloudTableClient tableClient;
		static CloudTable orderTable;
		static string orderTableName;

		[FunctionName("PostOrder")]
		public static async Task<IActionResult> Run(
				[HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
				ILogger log,
				ExecutionContext context) {
			var config = new ConfigurationBuilder()
				.SetBasePath(context.FunctionAppDirectory)
				.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();

			var storageName = config["AzureStorageName"];
			var accountKey = config["AzureAccountKey"];
			orderTableName = config["OrderTableName"];

			try {
				string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
				var order = JsonConvert.DeserializeObject<Order>(requestBody);

				log.LogInformation($"PostOrder,{DateTime.Now},orderId={order.OrderId}");
				storageAccount = new CloudStorageAccount(
					new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
						storageName, accountKey), true);
				tableClient = storageAccount.CreateCloudTableClient();
				orderTable = tableClient.GetTableReference(orderTableName);

				await AddOrderToQueue(order);
				order.OrderStatus = OrderStatus.Queued;

				await AddOrderToTable(order, requestBody);
				await AddOrderToUser(order);
				await AddOrderToRestaurantCatelogs(order);

				return (ActionResult)new OkResult();
			} catch (Exception ex) {

			}

			return new BadRequestResult();
		}

		static async Task<RestaurantOrderCatelogsEntity> GetCatelogs(string restaurantId) {
			TableOperation retrieveOperation = TableOperation.Retrieve<RestaurantOrderCatelogsEntity>(RestaurantOrderCatelogsEntity.PartitionKeyLabel, restaurantId);
			TableResult retrievedResult = await orderTable.ExecuteAsync(retrieveOperation);

			RestaurantOrderCatelogsEntity orderCatelogsEntity = null;
			if (retrievedResult.Result != null) {
				orderCatelogsEntity = (RestaurantOrderCatelogsEntity)retrievedResult.Result;
			} else {
				orderCatelogsEntity = new RestaurantOrderCatelogsEntity(restaurantId) {
					JSON = JsonConvert.SerializeObject(new RestaurantOrderCatelogs())
				};
			}

			return orderCatelogsEntity;
		}

		static async Task<RestaurantOrderCatelogEntity> GetCatelog(string catelogId) {
			TableOperation retrieveOperation = TableOperation.Retrieve<RestaurantOrderCatelogEntity>(RestaurantOrderCatelogEntity.PartitionKeyLabel, catelogId);
			TableResult retrievedResult = await orderTable.ExecuteAsync(retrieveOperation);

			RestaurantOrderCatelogEntity orderCatelogEntity = null;
			if (retrievedResult.Result != null) {
				orderCatelogEntity = (RestaurantOrderCatelogEntity)retrievedResult.Result;
			}

			return orderCatelogEntity;
		}

		static async Task<RestaurantOrderCatelog> GetCurrentCatelog(string restaurantId) {
			var orderCatelogsEntity = await GetCatelogs(restaurantId);
			var orderCatelogs = JsonConvert.DeserializeObject<RestaurantOrderCatelogs>(orderCatelogsEntity.JSON);

			bool newCatelog = false;
			int catelogsCount = orderCatelogs.CatelogIds.Count;
			RestaurantOrderCatelog orderCatelog = null;
			RestaurantOrderCatelogEntity orderCatelogEntity = null;
			if (catelogsCount == 0) {
				newCatelog = true;
			} else {
				orderCatelogEntity = await GetCatelog(orderCatelogs.CatelogIds[catelogsCount - 1]);
				orderCatelog = JsonConvert.DeserializeObject<RestaurantOrderCatelog>(orderCatelogEntity.JSON);
				if (orderCatelog.PeriodEnd.Date < DateTime.Now.Date)
					newCatelog = true;
			}

			if (newCatelog) {
				orderCatelog = new RestaurantOrderCatelog(restaurantId, orderCatelogs.PeriodDayCount);
				orderCatelogs.CatelogIds.Add(orderCatelog.CatelogId);

				orderCatelogsEntity.JSON = JsonConvert.SerializeObject(orderCatelogs);

				var upsertOrder = TableOperation.InsertOrReplace(orderCatelogsEntity);
				await orderTable.ExecuteAsync(upsertOrder);
			}

			return orderCatelog;
		}


		static async Task AddOrderToRestaurantCatelogs(Order order) {
			var catelogOrders = await GetCurrentCatelog(order.RestaurantId.ToString());
			var catelogOrdersEntity = new RestaurantOrderCatelogEntity(catelogOrders.CatelogId);

			catelogOrders.OrderIds.Add(order.OrderId.ToString());
			catelogOrdersEntity.JSON = JsonConvert.SerializeObject(catelogOrders);

			var upsertCatelog = TableOperation.InsertOrReplace(catelogOrdersEntity);
			await orderTable.ExecuteAsync(upsertCatelog);
		}

		/// <summary>
		/// Gets the user orders and creates a new one if none exists.
		/// </summary>
		/// <param name="userId">The user ID</param>
		/// <returns></returns>
		static async Task<UserOrdersEntity> GetUserOrders(string userId) {
			TableOperation retrieveOperation = TableOperation.Retrieve<UserOrdersEntity>(UserOrdersEntity.PartitionKeyLabel, userId);
			TableResult retrievedResult = await orderTable.ExecuteAsync(retrieveOperation);

			UserOrdersEntity userOrdersEntity = null;
			if (retrievedResult.Result != null) {
				userOrdersEntity = (UserOrdersEntity)retrievedResult.Result;
			} else {
				userOrdersEntity = new UserOrdersEntity(userId) {
					JSON = JsonConvert.SerializeObject(new UserOrders(userId))
				};
			}

			return userOrdersEntity;
		}

		/// <summary>
		/// Adds the order to the list of orders a user has made
		/// </summary>
		/// <param name="order">The order the user made</param>
		static async Task AddOrderToUser(Order order) {
			var userOrdersEntity = await GetUserOrders(order.UserId.ToString());
			var userOrders = JsonConvert.DeserializeObject<UserOrders>(userOrdersEntity.JSON);

			userOrders.OrderIds.Add(order.OrderId.ToString());
			userOrdersEntity.JSON = JsonConvert.SerializeObject(userOrders);

			var upsertOrder = TableOperation.InsertOrReplace(userOrdersEntity);
			await orderTable.ExecuteAsync(upsertOrder);
		}

		static async Task AddOrderToTable(Order order, string orderJSON) {
			var orderEntity = new OrderEntity(order.OrderId.ToString(), orderJSON);
			var insertOrder = TableOperation.Insert(orderEntity);

			await orderTable.ExecuteAsync(insertOrder);
		}

		static async Task AddOrderToQueue(Order order) {
			var queueClient = storageAccount.CreateCloudQueueClient();
			var queue = queueClient.GetQueueReference($"orders-{order.RestaurantId}");
			await queue.CreateIfNotExistsAsync();
			var message = new CloudQueueMessage(order.OrderId.ToString());
			await queue.AddMessageAsync(message);
		}

		/// <summary>
		/// Sends the push notification to the restaurant device via OneSignal
		/// </summary>
		static void SendNotification(string restaurantPlayerId) {
			var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

			request.KeepAlive = true;
			request.Method = "POST";
			request.ContentType = "application/json; charset=utf-8";

			//byte[] byteArray = Encoding.UTF8.GetBytes("{"
			//										+ "\"app_id\": \"fe020159-a228-44d2-96af-8dc46096a219\","
			//										+ "\"headings\": {\"en\": \"ORDER UP\"}"
			//										+ "\"contents\": {\"en\": \"There's an order.\"},"
			//										+ $"\"include_player_ids\": [\"{restaurantPlayerId}\"]}}");
			byte[] byteArray = Encoding.UTF8.GetBytes("{"
										+ "\"app_id\": \"fe020159-a228-44d2-96af-8dc46096a219\","
										+ "\"contents\": {\"en\": \"Order Received\"},"
										+ $"\"include_player_ids\": [\"{restaurantPlayerId}\"]}}");

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
	}
}
