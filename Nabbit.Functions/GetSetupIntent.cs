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

namespace Nabbit.Functions {
	public static class GetSetupIntent {
		[FunctionName("GetSetupIntent")]
		public static async Task<IActionResult> Run (
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetSetupIntent")] HttpRequest req,
			ILogger log,
			ExecutionContext context) {
			var config = new ConfigurationBuilder()
				.SetBasePath(context.FunctionAppDirectory)
				.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();

			var stripeKey = config["StripeSecretKey"];
			StripeConfiguration.ApiKey = stripeKey;

			try {
				var setupOptions = new SetupIntentCreateOptions {
					Usage = "on_session",
				};
				var setupService = new SetupIntentService();
				var setupIntent = await setupService.CreateAsync(setupOptions);

				return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(setupIntent));
			} catch (Exception e) {
				return new BadRequestObjectResult(e.Message);
			}
		}
	}
}
