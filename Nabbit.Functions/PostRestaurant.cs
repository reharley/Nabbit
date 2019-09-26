using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nabbit.Models;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;

namespace Nabbit.Functions {
	public static class PostRestaurant {
		[FunctionName("PostRestaurant")]
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
			var restaurantTableName = config["RestaurantTableName"];

			try {
				string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
				var restaurant = JsonConvert.DeserializeObject<Restaurant>(requestBody);

				log.LogInformation($"PostRestaurant,{DateTime.Now},restuarantId={restaurant.RestaurantId}");
				CloudStorageAccount storageAccount = new CloudStorageAccount(
					new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
						storageName, accountKey), true);
				CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
				CloudTable restaurantTable = tableClient.GetTableReference(restaurantTableName);

				RestaurantEntity restEntity = new RestaurantEntity(restaurant.RestaurantId.ToString(), requestBody);
				restEntity.ETag = "*";
				TableOperation update = TableOperation.Replace(restEntity);

				await restaurantTable.ExecuteAsync(update);
				return (ActionResult)new OkResult();
			} catch (Exception ex) {
				return new BadRequestObjectResult(ex.Message);
			}
		}
	}
}
