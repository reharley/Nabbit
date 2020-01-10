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
	public static class GetRestaurants {
		[FunctionName("GetRestaurants")]
		public static async Task<IActionResult> Run(
				[HttpTrigger(AuthorizationLevel.Function,
				"get",
				Route = "userId/{userId}/schoolId/{schoolId}")] HttpRequest req,
				string userId,
				string schoolId,
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

			log.LogInformation($"GetSchools,{DateTime.Now},userId={userId},schoolId={schoolId}");
			CloudStorageAccount storageAccount = new CloudStorageAccount(
				new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
					storageName, accountKey), true);
			CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
			CloudTable restaurantTable = tableClient.GetTableReference(restaurantTableName);

			// get the desired school object
			TableOperation retrieveOperation = TableOperation.Retrieve<SchoolEntity>(SchoolEntity.PartionKeyLabel, schoolId);
			TableResult retrievedResult = await restaurantTable.ExecuteAsync(retrieveOperation);

			SchoolEntity schoolEntity = null;
			if (retrievedResult.Result != null)
				schoolEntity = (SchoolEntity)retrievedResult.Result;
			else
				return new NotFoundResult();

			School school = JsonConvert.DeserializeObject<School>(schoolEntity.JSON);

			// create the query filter for the partition key
			string pkFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, RestaurantEntity.PartitionKeyLabel);

			// create the query filter for each restaurant id related to the school object
			List<string> rkFilters = new List<string>();
			foreach (var restaurantId in school.RestaurantIds)
				rkFilters.Add(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, restaurantId.ToString()));

			//  combine query filters
			string rkFilter = rkFilters[0];
			for (int i = 1; i < rkFilters.Count; i++)
				rkFilter = TableQuery.CombineFilters(rkFilter, TableOperators.Or, rkFilters[i]);

			string combinedFilter = TableQuery.CombineFilters(pkFilter, TableOperators.And, rkFilter);

			TableQuery<RestaurantEntity> query = new TableQuery<RestaurantEntity>().Where(combinedFilter);
			List<RestaurantEntity> restaurants = new List<RestaurantEntity>();
			
			TableContinuationToken token = null;
			do {
				TableQuerySegment<RestaurantEntity> resultSegment = await restaurantTable.ExecuteQuerySegmentedAsync(query, token);
				token = resultSegment.ContinuationToken;

				restaurants.AddRange(resultSegment.Results);
			} while (token != null);

			return new OkObjectResult(restaurants);
		}
	}
}
