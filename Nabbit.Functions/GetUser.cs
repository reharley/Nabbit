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
    public static class GetUser {

        [FunctionName("GetUser")]
        public static async Task<IActionResult> Run (
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetUser/userId/{userId}")] HttpRequest req,
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
            var userTableName = config["UserTableName"];

            try {
                log.LogInformation($"GetUser,{DateTime.Now},user={userId}");
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

                return new OkObjectResult(userEntity.JSON);
            } catch (Exception e) {
                return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }

        }
    }
}
