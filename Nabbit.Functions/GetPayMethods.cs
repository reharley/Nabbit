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
	public static class GetPayMethods {
		[FunctionName("GetPayMethods")]
		public static async Task<IActionResult> Run (
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetPayMethods/customerId/{customerId}")] HttpRequest req,
			string customerId,
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
				var service = new PaymentMethodService();
				var options = new PaymentMethodListOptions {
					Customer = customerId,
					Type = "card",
				};
				var paymentMethods = await service.ListAsync(options);
				return (ActionResult)new OkObjectResult(paymentMethods.ToJson());
			} catch (Exception e) {
				return new BadRequestObjectResult(e.Message);
			}
		}
	}
}
