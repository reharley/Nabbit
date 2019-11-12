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

			BindingContext = viewModel = new AccountViewModel();
		}


		protected override void OnAppearing () {
			viewModel.UserInfo = LocalGlobals.User;
		}

		private async void OrderHistoryClicked (object sender, EventArgs e) {
			await Navigation.PushAsync(new OrderHistoryPage());
		}

		private async void PaymentMethodsClicked (object sender, EventArgs e) {
			await Navigation.PushAsync(new PaymentMethodsPage());
		}

		private async void EditProfileClicked (object sender, EventArgs e) {
			await Navigation.PushAsync(new PaymentMethodsPage());
		}

		private async void LogoutClicked (object sender, EventArgs e) {
			Auth.SignOut();
			LocalGlobals.Logout();

			await Navigation.PushModalAsync(new SignInPage());
		}
	}
}