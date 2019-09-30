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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Nabbit.Models;
using System.Collections.Generic;

namespace Nabbit.Functions.Properties {
	public static class GetRestaurantOrders {
		static CloudTable orderTable;

		[FunctionName("GetRestaurantOrders")]
		public static async Task<IActionResult> Run (
			[HttpTrigger(AuthorizationLevel.Function, 
			"get", Route = "GetRestOrders/{restaurantId}")] HttpRequest req,
			string restaurantId,
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
				log.LogInformation($"GetRestOrders,{DateTime.Now},user={restaurantId}");
				var storageAccount = new CloudStorageAccount(
					new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
						storageName, accountKey), true);
				var tableClient = storageAccount.CreateCloudTableClient();
				orderTable = tableClient.GetTableReference(orderTableName);

				var orders = await CollectRestOrders(restaurantId);
				if (orders == null)
					return new OkObjectResult("none");

				return new OkObjectResult(JsonConvert.SerializeObject(orders));
			} catch (Exception e) {
				return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
			}
		}

		static async Task<List<Order>> CollectRestOrders(string restaurantId) {
			var orderCatelog = await GetCurrentCatelog(restaurantId);
			if (orderCatelog == null)
				return null;

			// create the query filter for the partition key
			string pkFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, OrderEntity.PartitionKeyLabel);

			// create the query filter for each restaurant id related to the school object
			List<string> rkFilters = new List<string>();
			foreach (var orderId in orderCatelog.OrderIds)
				rkFilters.Add(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, orderId));

			//  combine query filters
			if (rkFilters.Count == 0)
				return null;
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

		static async Task<RestaurantOrderCatelogsEntity> GetCatelogs (string restaurantId) {
			TableOperation retrieveOperation = TableOperation.Retrieve<RestaurantOrderCatelogsEntity>(RestaurantOrderCatelogsEntity.PartitionKeyLabel, restaurantId);
			TableResult retrievedResult = await orderTable.ExecuteAsync(retrieveOperation);

			RestaurantOrderCatelogsEntity orderCatelogsEntity = null;
			if (retrievedResult.Result != null) {
				orderCatelogsEntity = (RestaurantOrderCatelogsEntity)retrievedResult.Result;
			}

			return orderCatelogsEntity;
		}

		static async Task<RestaurantOrderCatelogEntity> GetCatelog (string catelogId) {
			TableOperation retrieveOperation = TableOperation.Retrieve<RestaurantOrderCatelogEntity>(RestaurantOrderCatelogEntity.PartitionKeyLabel, catelogId);
			TableResult retrievedResult = await orderTable.ExecuteAsync(retrieveOperation);

			RestaurantOrderCatelogEntity orderCatelogEntity = null;
			if (retrievedResult.Result != null) {
				orderCatelogEntity = (RestaurantOrderCatelogEntity)retrievedResult.Result;
			}

			return orderCatelogEntity;
		}

		static async Task<RestaurantOrderCatelog> GetCurrentCatelog (string restaurantId) {
			var orderCatelogsEntity = await GetCatelogs(restaurantId);
			if (orderCatelogsEntity == null)
				return null;
			var orderCatelogs = JsonConvert.DeserializeObject<RestaurantOrderCatelogs>(orderCatelogsEntity.JSON);

			int catelogsCount = orderCatelogs.CatelogIds.Count;
			RestaurantOrderCatelog orderCatelog = null;
			RestaurantOrderCatelogEntity orderCatelogEntity = null;
			if (catelogsCount == 0) {
				return null;
			} else {
				orderCatelogEntity = await GetCatelog(orderCatelogs.CatelogIds[catelogsCount - 1]);
				orderCatelog = JsonConvert.DeserializeObject<RestaurantOrderCatelog>(orderCatelogEntity.JSON);
			}

			return orderCatelog;
		}
	}
}
