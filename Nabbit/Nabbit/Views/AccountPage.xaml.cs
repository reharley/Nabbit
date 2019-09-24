using Microsoft.AppCenter.Auth;
using Nabbit.Services;
using Nabbit.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.SecureStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nabbit.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AccountPage : ContentPage {
		AccountViewModel viewModel;
		public AccountPage () {
			InitializeComponent();
			test.Text = "Click to sign in";

			LocalGlobals.GetUser().Wait();
			BindingContext = viewModel = new AccountViewModel();
			test.Text = "Logged In";
		}


		protected override void OnAppearing () {
			viewModel.UserInfo = LocalGlobals.User;
			firstName.Text = "Welcome, " + viewModel.UserInfo.FirstName;
		}

		private async void OrderHistoryClicked (object sender, EventArgs e) {
			await Shell.Current.GoToAsync("orderHistory");
		}

		private async void PaymentMethodsClicked (object sender, EventArgs e) {
			await Shell.Current.GoToAsync("paymentMethods");
		}

		private async void TestChargeClicked (object sender, EventArgs e) {
			
		}
	}
}