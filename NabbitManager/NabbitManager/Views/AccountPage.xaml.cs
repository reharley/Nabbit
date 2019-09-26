using Microsoft.AppCenter.Auth;
using Nabbit.ViewModels;
using Newtonsoft.Json;
using Plugin.SecureStorage;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NabbitManager.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AccountPage : ContentPage {
		AccountViewModel viewModel;

		public AccountPage() {
			InitializeComponent();
			test.Text = "Click to sign in";

			BindingContext = viewModel = new AccountViewModel();
			//if (viewModel.UserInfo.LoggedIn) {
			//	test.Text = "Logged In";
			//	firstName.Text = viewModel.UserInfo.FirstName;
			//	lastName.Text = viewModel.UserInfo.LastName;
			//}
		}

		async Task SignIn() {
			try {
				// Sign-in succeeded.
				UserInformation userInfo = await Auth.SignInAsync();
				CrossSecureStorage.Current.SetValue("UserToken", JsonConvert.SerializeObject(userInfo));

				test.Text = "Success!";
			} catch (Exception e) {
				// Do something with sign-in failure.
				test.Text = "Failure!";
			}
		}

		private async void SignInClicked(object sender, EventArgs e) {
			await SignIn();
		}

		private async void HoursClicked (object sender, EventArgs e) {
			await Navigation.PushAsync(new RestaurantDetailsPage());
		}
	}
}