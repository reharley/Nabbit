using System;
using System.Collections.Generic;
using Nabbit.Services;
using Xamarin.Forms;

namespace Nabbit.Views {
	public partial class PaymentInfoPage : ContentPage {
		public PaymentInfoPage () {
			InitializeComponent();
			webView.Source = DependencyService.Get<IBaseUrl>().Get() + "/payment.html";
		}
	}
}
