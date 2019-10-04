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

namespace Nabbit.Functions {
	public static class DeleteQueueOrder {
		static CloudStorageAccount storageAccount;
		static CloudTableClient tableClient;
		static CloudTable orderTable;

		[FunctionName("DeleteQueueOrder")]
		public static async Task<IActionResult> Run (
			[HttpTrigger(AuthorizationLevel.Function,
			"get",
			Route = "DeleteQueueOrder/restaurantId/{restaurantId}/orderId/{orderId}")] HttpRequest req,
			string restaurantId,
			string orderId,
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
				storageAccount = new CloudStorageAccount(
					new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
						storageName, accountKey), true);
				tableClient = storageAccount.CreateCloudTableClient();
				orderTable = tableClient.GetTableReference(orderTableName);

				await RemoveOrderFromQueue(restaurantId, orderId);
				await UpdateOrder(orderId);

				return new OkResult();
			} catch (Exception ex) {
				log.LogInformation($"GetQueueOrders,{DateTime.Now},restaurantId={restaurantId},error={ex.Message}");
			}

			return new BadRequestResult();
		}

		static async Task RemoveOrderFromQueue (string restaurantId, string orderId) {
			var queueClient = storageAccount.CreateCloudQueueClient();
			var queue = queueClient.GetQueueReference($"orders-{restaurantId}");
			if (queue == null)
				return;

			await queue.FetchAttributesAsync();
			int count = queue.ApproximateMessageCount.GetValueOrDefault();
			while (count > 0) {
				var messages = await queue.GetMessagesAsync(count % 32);
				foreach (var message in messages) {
					if (message.AsString == orderId) {
						await queue.DeleteMessageAsync(message);
						return;
					}
				}

				await queue.FetchAttributesAsync();
				count = queue.ApproximateMessageCount.GetValueOrDefault();
			}
		}

		static async Task UpdateOrder(string orderId) {
			var tableOperation = TableOperation.Retrieve<OrderEntity>(OrderEntity.PartitionKeyLabel, orderId);
			var result = await orderTable.ExecuteAsync(tableOperation);
			var orderEntity = result.Result as OrderEntity;

			if (orderEntity != null) {
				var order = JsonConvert.DeserializeObject<Order>(orderEntity.JSON);
				order.OrderStatus = OrderStatus.Complete;

				orderEntity.JSON = JsonConvert.SerializeObject(order);
				var updateOperation = TableOperation.Replace(orderEntity);

				var updateResult = await orderTable.ExecuteAsync(updateOperation);
			}
		}
	}
}
