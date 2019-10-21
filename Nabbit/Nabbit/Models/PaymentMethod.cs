using System;
namespace Nabbit.Models {
	public class PaymentMethod {
		public PaymentMethod () {
		}

		public string PaymentMethodId { get; set; }

		public string CardNumber { get; set; }
		public string CardLastFour { get; set; }
		public string CardType { get; set; }
		public string PersonName { get; set; }
		public string CardExpire { get; set; }
		public string MonthExpire { get; set; }
		public string YearExpire { get; set; }
		public string CVV { get; set; }
		public string Address1 { get; set; }
		public string Country { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string BillingAddress { get; set; }

		public void BuildBillingAddress () {
			BillingAddress = PersonName + "\n" +
							 Address1 + "\n" + Address2 + "\n" +
							 City + " " + State + "\n" +
							 Zip;
			CardExpire = MonthExpire + "/" + YearExpire;
		}
	}
}
