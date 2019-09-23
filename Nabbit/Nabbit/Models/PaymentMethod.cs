using System;
namespace Nabbit.Models {
	public class PaymentMethod {
		public PaymentMethod () {
		}

		public Guid PaymentMethodId { get; set; }
		public string CardType { get; set; }
		public string CardLastFour { get; set; }
		public string PersonName { get; set; }
		public string CardExpire { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string BillingAddress { get; set; }
	}
}
