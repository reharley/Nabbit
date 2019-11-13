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
using System.Threading;
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
			base.OnAppearing();
			UpdatePage();
		}

		void UpdatePage () {
			viewModel.UserInfo = LocalGlobals.User;

			if (LocalGlobals.User.LoggedIn == false) {
				orderHistoryButton.IsEnabled = false;
				payMethodsButton.IsEnabled = false;
				logoutButton.Text = "Login";
			} else {
				viewModel.UserInfo = LocalGlobals.User;
				orderHistoryButton.IsEnabled = true;
				payMethodsButton.IsEnabled = true;
				logoutButton.Text = "Logout";
			}
		}

		async Task SignIn () {
			var signInPage = new SignInPage();
			signInPage.Disappearing += UpdateEventHandler;
			await Navigation.PushModalAsync(signInPage);
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

		void UpdateEventHandler (object sender, EventArgs e) {
			UpdatePage();
		}

		private async void LogoutClicked (object sender, EventArgs e) {
			if (LocalGlobals.User.LoggedIn == false) {
				SignIn();
			} else {
				Auth.SignOut();
				LocalGlobals.Logout();
				UpdatePage();
			}
		}
	}
}