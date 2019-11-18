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
using Stripe;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Nabbit.Models;

namespace Nabbit.Functions {
	public static class CreateCustomer {
		[FunctionName("CreateCustomer")]
		public static async Task<IActionResult> Run (
			[HttpTrigger(AuthorizationLevel.Function, "get",
			Route = "CreateCustomer/userId/{userId}")] HttpRequest req,
			string userId,
			ILogger log,
			ExecutionContext context) {
			var config = new ConfigurationBuilder()
				.SetBasePath(context.FunctionAppDirectory)
				.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();

			var stripeKey = config["StripeSecretKey"];
			var storageName = config["AzureStorageName"];
			var userTableName = config["UserTableName"];
			var accountKey = config["AzureAccountKey"];

			try {
				log.LogInformation($"CreateCustomer,{DateTime.Now},user={userId}");
				var storageAccount = new CloudStorageAccount(
					new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
						storageName, accountKey), true);
				var tableClient = storageAccount.CreateCloudTableClient();
				var userTable = tableClient.GetTableReference(userTableName);


				TableOperation retrieveOperation = TableOperation.Retrieve<UserEntity>(UserEntity.PartitionKeyLabel, userId);
				TableResult retrievedResult = await userTable.ExecuteAsync(retrieveOperation);
				UserEntity userEntity = null;
				if (retrievedResult.Result != null) {
					userEntity = (UserEntity)retrievedResult.Result;
				}
				if (retrievedResult.Result == null) {
					var error = "user not found";
					log.LogInformation($"CreateCustomer,{DateTime.Now},user={userId},error={error}");
					return new BadRequestObjectResult(error);
				}

				var user = JsonConvert.DeserializeObject<User>(userEntity.JSON);

				StripeConfiguration.ApiKey = stripeKey;
				var options = new CustomerCreateOptions {
					Name = user.FirstName + " " + user.LastName,
					Email = user.Email
				};

				var service = new CustomerService();
				Customer customer = service.Create(options);

				return (ActionResult)new OkObjectResult(customer.Id);
			} catch (Exception e) {
				log.LogInformation($"CreateCustomer,{DateTime.Now},user={userId},error={e.Message}");
				return new BadRequestObjectResult(e.Message);
			}
		}
	}
}
