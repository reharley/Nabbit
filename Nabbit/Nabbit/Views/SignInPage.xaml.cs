using Microsoft.AppCenter.Auth;
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
	public partial class SignInPage : ContentPage {
		public SignInPage() {
			InitializeComponent();
		}

		async Task SignIn() {
			try {
				// Sign-in succeeded.
				UserInformation userInfo = await Auth.SignInAsync();
				CrossSecureStorage.Current.SetValue("UserToken", userInfo.IdToken);

				signInLabel.Text = "Login Success!";
				await Navigation.PopModalAsync();
			} catch (Exception e) {
				// Do something with sign-in failure.
				signInLabel.Text = "Login Failed... Please try again.";
			}
		}

		private async void SignInClicked(object sender, EventArgs e) {
			await SignIn();
		}
	}
}