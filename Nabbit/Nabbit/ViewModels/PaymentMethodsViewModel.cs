using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Nabbit.Models;
using Nabbit.Services;
using Newtonsoft.Json.Linq;

namespace Nabbit.ViewModels {
	public class PaymentMethodsViewModel : BaseViewModel {
		List<PaymentMethod> paymentMethods;
		public List<PaymentMethod> PaymentMethods {
			get {
				if (paymentMethods == null)
					paymentMethods = new List<PaymentMethod>();

				return paymentMethods;
			}
			set {
				SetProperty(ref paymentMethods, value);
			}
		}

		public PaymentMethodsViewModel () {
		}
	}
}
