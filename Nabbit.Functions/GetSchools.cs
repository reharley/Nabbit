using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Nabbit.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Nabbit.Functions {
	public static class GetSchools {
		[FunctionName("GetSchools")]
		public static async Task<IActionResult> Run(
				[HttpTrigger(AuthorizationLevel.Function,
					"get",
					Route = "userId/{userId}")]
				HttpRequest req,
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
			var restaurantTableName = config["RestaurantTableName"];

			log.LogInformation($"GetSchools,{DateTime.Now},userId={userId}");

			CloudStorageAccount storageAccount = new CloudStorageAccount(
				new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
					storageName, accountKey), true);
			CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
			CloudTable restaurantTable = tableClient.GetTableReference(restaurantTableName);

			// construct query to get all schools
			TableQuery<SchoolEntity> query = new TableQuery<SchoolEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, SchoolEntity.PartionKeyLabel));
			List<SchoolEntity> schools = new List<SchoolEntity>();

			// get all the schools available
			TableContinuationToken token = null;
			do {
				TableQuerySegment<SchoolEntity> resultSegment = await restaurantTable.ExecuteQuerySegmentedAsync(query, token);
				token = resultSegment.ContinuationToken;

				schools.AddRange(resultSegment.Results);
			} while (token != null);

			return new OkObjectResult(schools);
		}
	}
}
