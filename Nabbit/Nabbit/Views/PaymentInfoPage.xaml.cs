using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Nabbit.Services;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace Nabbit.Views {
	public partial class PaymentInfoPage : ContentPage {
		public PaymentInfoPage () {
			InitializeComponent();
			webView.Source = DependencyService.Get<IBaseUrl>().Get() + "/payment.html";
			var user = LocalGlobals.User;
			webView.Eval($"passParams({user.CustomerId.ToString()}, {user.Email});");
		}

		async void WebViewNavigating(object sender, WebNavigatingEventArgs e) {
			if (e.Url.Contains("payment_method")) {
				var paymentMethodId = e.Url.Split('?')[1];
				LocalGlobals.User.PaymentMethodId = Guid.Parse(paymentMethodId);
				e.Cancel = true;

				await Navigation.PopAsync();
			}
		}
	}
}
