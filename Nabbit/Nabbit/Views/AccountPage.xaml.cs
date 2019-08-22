using Microsoft.AppCenter.Auth;
using Nabbit.Services;
using Nabbit.ViewModels;
using Newtonsoft.Json;
using Plugin.SecureStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nabbit.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AccountPage : ContentPage {
		AccountViewModel viewModel;
		public AccountPage() {
			InitializeComponent();
			test.Text = "Click to sign in";

			BindingContext = viewModel = new AccountViewModel();
			if (viewModel.UserInfo.LoggedIn) {
				test.Text = "Logged In";
			} else {
				PushSignInPage();
			}
		}
		
		async void PushSignInPage() {
			await App.Current.MainPage.Navigation.PushModalAsync(new SignInPage());
			viewModel.UserInfo = LocalGlobals.User;
			firstName.Text = "Welcome, " + viewModel.UserInfo.FirstName;
		}

		protected override void OnAppearing() {
			viewModel.UserInfo = LocalGlobals.User;
			firstName.Text = "Welcome, " + viewModel.UserInfo.FirstName;
		}

		private async void OrderHistoryClicked(object sender, EventArgs e) {
			await App.Current.MainPage.Navigation.PushModalAsync(new OrderHistoryPage());
		}
	}
}