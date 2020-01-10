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

namespace Nabbit.Functions {
	public static class PingRestaurant {
		[FunctionName("PingRestaurant")]
		public static async Task<IActionResult> Run (
				[HttpTrigger(AuthorizationLevel.Function, "get",
					Route = "PingRestaurant/restaurantId/{restaurantId}/deviceId/{deviceId}")]
					HttpRequest req,
				string restaurantId, string deviceId,
				ILogger log,
				ExecutionContext context) {
			var config = new ConfigurationBuilder()
				.SetBasePath(context.FunctionAppDirectory)
				.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();

			var storageName = config["AzureStorageName"];
			var accountKey = config["AzureAccountKey"];
			var restaurantTableName = config["RestaurantTableName"];

			try {
				log.LogInformation($"PostRestaurant,{DateTime.Now},restuarantId={restaurantId}");
				CloudStorageAccount storageAccount = new CloudStorageAccount(
					new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
						storageName, accountKey), true);
				CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
				CloudTable restaurantTable = tableClient.GetTableReference(restaurantTableName);

				TableOperation retrieveOperation = TableOperation.Retrieve<RestaurantEntity>(RestaurantEntity.PartitionKeyLabel, restaurantId);
				TableResult retrievedResult = await restaurantTable.ExecuteAsync(retrieveOperation);
				RestaurantEntity restaurantEntity = null;
				if (retrievedResult.Result != null) {
					restaurantEntity = (RestaurantEntity)retrievedResult.Result;
				}

				var restaurant = JsonConvert.DeserializeObject<Restaurant>(restaurantEntity.JSON);
				if (restaurant.PrinterId.ToString() != deviceId) {
					return new OkObjectResult(JsonConvert.SerializeObject(new PingRestaurantResponse()));
				}
				var updateRestaurant = restaurant.UpdateRestaurant;

				restaurant.UpdateRestaurant = false;
				restaurant.LastPing = DateTime.Now;

				var restaurantJson = JsonConvert.SerializeObject(restaurant);
				RestaurantEntity restEntity = new RestaurantEntity(restaurant.RestaurantId.ToString(), restaurantJson);
				restEntity.ETag = "*";
				TableOperation update = TableOperation.Replace(restEntity);

				await restaurantTable.ExecuteAsync(update);

				return new OkObjectResult(JsonConvert.SerializeObject(new PingRestaurantResponse(isDevice: true, updateRestaurant)));
			} catch (Exception ex) {
				return new BadRequestObjectResult(ex.Message);
			}
		}
	}
}
