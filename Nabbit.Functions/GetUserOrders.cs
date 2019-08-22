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
using System.Collections.Generic;

namespace Nabbit.Functions {
	public static class GetUserOrders {
		static CloudTable orderTable;
		[FunctionName("GetUserOrders")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetUserOrders/userId/{userId}")] HttpRequest req,
			string userId,
			ILogger log,
			ExecutionContext context) {
			var config = new ConfigurationBuilder()
				.SetBasePath(context.FunctionAppDirectory)
				.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();

			var storageName = config["AzureStorageName"];
			var accountKey = config["AzureAccountKey"];
			var orderTableName = config["OrderTableName"];

			try {
				log.LogInformation($"GetUserOrders,{DateTime.Now},user={userId}");
				var storageAccount = new CloudStorageAccount(
					new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
						storageName, accountKey), true);
				var tableClient = storageAccount.CreateCloudTableClient();
				orderTable = tableClient.GetTableReference(orderTableName);

				var orders = await GetUserOrdersList(userId);
				if (orders == null)
					return new OkObjectResult("none");

				return new OkObjectResult(JsonConvert.SerializeObject(orders));
			} catch (Exception e) {

			}

			return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
		}

		static async Task<UserOrdersEntity> GetUserOrdersEntity(string userId) {
			TableOperation retrieveOperation = TableOperation.Retrieve<UserOrdersEntity>(UserOrdersEntity.PartitionKeyLabel, userId);
			TableResult retrievedResult = await orderTable.ExecuteAsync(retrieveOperation);

			UserOrdersEntity orderCatelogsEntity = null;
			if (retrievedResult.Result != null) {
				orderCatelogsEntity = (UserOrdersEntity)retrievedResult.Result;
			}

			return orderCatelogsEntity;
		}

		static async Task<List<Order>> GetUserOrdersList(string userId) {
			var userOrdersEntity = await GetUserOrdersEntity(userId);
			if (userOrdersEntity == null)
				return null;

			var userOrders = JsonConvert.DeserializeObject<UserOrders>(userOrdersEntity.JSON);
			// create the query filter for the partition key
			string pkFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, OrderEntity.PartitionKeyLabel);

			// create the query filter for each restaurant id related to the school object
			List<string> rkFilters = new List<string>();
			foreach (var orderId in userOrders.OrderIds)
				rkFilters.Add(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, orderId));

			//  combine query filters
			string rkFilter = rkFilters[0];
			for (int i = 1; i < rkFilters.Count; i++)
				rkFilter = TableQuery.CombineFilters(rkFilter, TableOperators.Or, rkFilters[i]);

			string combinedFilter = TableQuery.CombineFilters(pkFilter, TableOperators.And, rkFilter);

			TableQuery<OrderEntity> query = new TableQuery<OrderEntity>().Where(combinedFilter);
			List<OrderEntity> orderEntities = new List<OrderEntity>();


			TableContinuationToken token = null;
			do {
				TableQuerySegment<OrderEntity> resultSegment = await orderTable.ExecuteQuerySegmentedAsync(query, token);
				token = resultSegment.ContinuationToken;

				orderEntities.AddRange(resultSegment.Results);
			} while (token != null);

			var orders = new List<Order>();
			foreach (var entity in orderEntities)
				orders.Add(JsonConvert.DeserializeObject<Order>(entity.JSON));

			return orders;
		}
	}
}
