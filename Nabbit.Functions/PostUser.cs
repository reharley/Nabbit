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
    public static class PostUser {
        [FunctionName("PostUser")]
        public static async Task<IActionResult> Run (
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "PostUser")] HttpRequest req,
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
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var user = JsonConvert.DeserializeObject<User>(requestBody);

                log.LogInformation($"PostUser,{DateTime.Now},userId={user.UserId}");
                var storageAccount = new CloudStorageAccount(
                    new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                        storageName, accountKey), true);
                var tableClient = storageAccount.CreateCloudTableClient();
                var userTable = tableClient.GetTableReference(userTableName);
				await userTable.CreateIfNotExistsAsync();
                var upsertUser = TableOperation.InsertOrReplace(new UserEntity(user.UserId.ToString(), requestBody));

                await userTable.ExecuteAsync(upsertUser);

                return (ActionResult)new OkResult();
            } catch (Exception ex) {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
