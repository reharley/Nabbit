using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stripe;
using Microsoft.Extensions.Configuration;

namespace Nabbit.Functions {
	public static class GetPayIntent {
		[FunctionName("GetPayIntent")]
		public static async Task<IActionResult> Run (
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetPayIntent/amount/{amount}/customerId/{customerId}/paymentMethodId/{paymentMethodId}")] HttpRequest req,
			string amount, string customerId, string paymentMethodId,
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
				var service = new PaymentIntentService();
				var options = new PaymentIntentCreateOptions {
					Amount = long.Parse(amount),
					Currency = "usd",
					Customer = customerId,
					PaymentMethod = paymentMethodId,
				};
				var intent = await service.CreateAsync(options);
				return (ActionResult)new OkObjectResult(intent.ToJson());
			} catch (Exception e) {
				return new BadRequestObjectResult(e.Message);
			}
		}
	}
}
