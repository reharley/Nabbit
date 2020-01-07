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
	public static class AttachUserPayment {
		[FunctionName("AttachUserPayment")]
		public static async Task<IActionResult> Run (
			[HttpTrigger(AuthorizationLevel.Function,
			"get",
			Route = "AttachUserPayment/custId/{custId}/payId/{payId}")] HttpRequest req,
			string custId, string payId,
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
				var payService = new PaymentMethodService();
				var options = new PaymentMethodAttachOptions {
					Customer = custId,
				};

				var completePaymentMethod = await payService.AttachAsync(payId, options);
				return (ActionResult)new OkResult();
			} catch (Exception e) {
				return new BadRequestObjectResult(e.Message);
			}
		}
	}
}
