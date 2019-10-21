using System;
using System.Collections.Generic;
using Nabbit.Models;

namespace Nabbit.ViewModels {
	public class PaymentMethodEditViewModel : BaseViewModel {
		public PaymentMethod PayMethod { get; set; }

		string cardNumber;
		public string CardNumber {
			get { return cardNumber; }
			set { SetProperty(ref cardNumber, value); }
		}

		string cardCvv;
		public string CardCvv {
			get { return cardCvv; }
			set { SetProperty(ref cardCvv, value); }
		}

		string cardExpirationDate;
		public string CardEpirationDate {
			get { return cardExpirationDate; }
			set { SetProperty(ref cardExpirationDate, value); }
		}

		public PaymentMethodEditViewModel () {
			PayMethod = new PaymentMethod();
		}

		public void SaveModel () {
			PayMethod.CardNumber = CardNumber.Trim(' ');
			PayMethod.CVV = CardCvv;
			var expDate = CardEpirationDate.Split('/');
			PayMethod.MonthExpire = expDate[0];
			PayMethod.YearExpire = expDate[1];
		}

		public void TestModel () {
			PayMethod = new PaymentMethod() {
				Address1 = "2211 Douglas Ave",
				Address2 = "Apt 209",
				City = "Bellingham",
				//CardNumber = "4242424242424242",
				//CardNumber = "4000000000000010",
				CardNumber = "4000000000000036",
				CVV = "123",
				MonthExpire = "12",
				YearExpire = "20",
				Country = "US",
				PersonName = "Cvc Fail",
				State = "WA",
				Zip = "98225"
			};
		}
	}
}
