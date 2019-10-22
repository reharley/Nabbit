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
	public static class DetachPayMethod {
		[FunctionName("DetachPayMethod")]
		public static async Task<IActionResult> Run (
			[HttpTrigger(AuthorizationLevel.Function,
			"get", Route = "DetachPayMethod/paymentMethodId/{paymentMethodId}")] HttpRequest req,
			string paymentMethodId,
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
				var options = new PaymentMethodDetachOptions();
				var paymentMethod = await service.DetachAsync(paymentMethodId, options);
				return (ActionResult)new OkResult();
			} catch (Exception e) {
				return new BadRequestObjectResult(e.Message);
			}
		}
	}
}
