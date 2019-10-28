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

namespace Nabbit.Functions {
	public static class GetPubKey {
		[FunctionName("GetPubKey")]
		public static async Task<IActionResult> Run (
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetPubKey")] HttpRequest req,
			ILogger log,
			ExecutionContext context) {
			var config = new ConfigurationBuilder()
				.SetBasePath(context.FunctionAppDirectory)
				.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();

			var stripeKey = config["StripePubKey"];

			if (stripeKey != null || stripeKey != "")
				return (ActionResult)new OkObjectResult(stripeKey);
			else
				return new BadRequestResult();
		}
	}
}
