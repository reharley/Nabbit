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

		async void WebViewNavigating (object sender, WebNavigatingEventArgs e) {
			if (e.Url.Contains("payment_method")) {
				e.Cancel = true;

				var paymentMethodId = e.Url.Split('?')[1];
				LocalGlobals.User.PaymentMethodIds.Add(Guid.Parse(paymentMethodId));
				await LocalGlobals.SaveUser();
				await App.Current.MainPage.Navigation.PopModalAsync();
			} else if (e.Url.Contains("cancel")) {
				await App.Current.MainPage.Navigation.PopModalAsync();
			}
		}
	}
}
