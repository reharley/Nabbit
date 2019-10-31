using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Nabbit.Models;
using Microsoft.WindowsAzure.Storage;

namespace Nabbit.Functions {
	public static class GetQueueOrders {
		static CloudStorageAccount storageAccount;
		static CloudTableClient tableClient;
		static CloudTable orderTable;

		[FunctionName("GetQueueOrders")]
		public static async Task<IActionResult> Run (
			[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "GetQueueOrders/restaurantId/{restaurantId}/allOrders/{allOrders}")] HttpRequest req,
			string restaurantId, string allOrders,
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
				log.LogInformation($"GetQueueOrders,{DateTime.Now},restaurantId={restaurantId}");
				string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
				var order = JsonConvert.DeserializeObject<List<string>>(requestBody);

				storageAccount = new CloudStorageAccount(
					new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
						storageName, accountKey), true);
				tableClient = storageAccount.CreateCloudTableClient();
				orderTable = tableClient.GetTableReference(orderTableName);

				var orders = await GetQueueOrderList(restaurantId, bool.Parse(allOrders));

				return new OkObjectResult(JsonConvert.SerializeObject(orders));
			} catch (Exception ex) {
				log.LogInformation($"GetQueueOrders,{DateTime.Now},restaurantId={restaurantId},error={ex.Message}");
			}

			return new BadRequestResult();
		}

		static async Task<List<string>> GetQueueOrderIdList (string restaurantId) {
			var queueClient = storageAccount.CreateCloudQueueClient();
			var queue = queueClient.GetQueueReference($"orders-{restaurantId}");
			if (queue == null)
				return null;

			await queue.FetchAttributesAsync();
			var orderIds = new List<string>();
			int count = queue.ApproximateMessageCount.GetValueOrDefault();
			while (count > 0) {
				var messages = await queue.GetMessagesAsync(count % 32);
				foreach (var message in messages) {
					orderIds.Add(message.AsString);
					await queue.DeleteMessageAsync(message);
				}

				await queue.FetchAttributesAsync();
				count = queue.ApproximateMessageCount.GetValueOrDefault();
			}

			return orderIds;
		}

		static async Task<List<string>> AddToLiveOrders (List<string> orderIds, string restaurantId, bool allOrders) {
			TableOperation retrieveOperation = TableOperation.Retrieve<LiveOrdersEntity>(LiveOrdersEntity.PartitionKeyLabel, restaurantId);
			TableResult retrievedResult = await orderTable.ExecuteAsync(retrieveOperation);
			LiveOrdersEntity userEntity = null;
			List<string> liveOrders = new List<string>();
			if (retrievedResult.Result != null) {
				userEntity = (LiveOrdersEntity)retrievedResult.Result;
				liveOrders = JsonConvert.DeserializeObject<List<string>>(userEntity.JSON);
			}

			liveOrders.AddRange(orderIds);

			var liveOrderEntity = new LiveOrdersEntity(restaurantId, JsonConvert.SerializeObject(liveOrders));
			var insertOrder = TableOperation.InsertOrReplace(liveOrderEntity);

			await orderTable.ExecuteAsync(insertOrder);

			if (allOrders)
				return liveOrders;

			return orderIds;
		}

		static async Task<List<Order>> GetQueueOrderList (string restaurantId, bool allOrders) {
			var orderIds = await GetQueueOrderIdList(restaurantId);
			if (orderIds == null)
				return null;
			else if (orderIds.Count == 0 && allOrders == false)
				return new List<Order>();

			orderIds = await AddToLiveOrders(orderIds, restaurantId, allOrders);
			if (orderIds.Count == 0)
				return new List<Order>();

			// create the query filter for the partition key
			string pkFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, OrderEntity.PartitionKeyLabel);

			// create the query filter for each restaurant id related to the school object
			List<string> rkFilters = new List<string>();
			foreach (var orderId in orderIds)
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
