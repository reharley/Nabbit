using Microsoft.AppCenter.Auth;
using Nabbit.Models;
using Nabbit.Services;
using Newtonsoft.Json.Linq;
using Plugin.SecureStorage;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nabbit.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SignInPage : ContentPage {
		public SignInPage () {
			InitializeComponent();
		}

		async Task SignIn () {
			try {
				// Sign-in succeeded.
				UserInformation userInfo = await Auth.SignInAsync();
				var tokenHandler = new JwtSecurityTokenHandler();
				try {
					var jwToken = tokenHandler.ReadJwtToken(userInfo.IdToken);
					LocalGlobals.User.FirstName = jwToken.Claims.FirstOrDefault(t => t.Type == "given_name").Value;
					LocalGlobals.User.LastName = jwToken.Claims.FirstOrDefault(t => t.Type == "family_name").Value;
					LocalGlobals.User.UserId = new Guid(jwToken.Claims.FirstOrDefault(t => t.Type == "oid").Value);
					LocalGlobals.User.Email = jwToken.Claims.FirstOrDefault(t => t.Type == "emails").Value;

					await LocalGlobals.GetUser();
				} catch (Exception e) {

				}


				

				if (LocalGlobals.User.PaymentMethodIds.Count == 0) {
					var payPage = new PaymentInfoPage();
					payPage.Disappearing += async (sender2, e2) => {
						signInLabel.Text = "Login Success!";
						await App.Current.MainPage.Navigation.PopModalAsync();
						App.Current.MainPage = new AppShell();
					};
					await App.Current.MainPage.Navigation.PushAsync(payPage);
				} else {
					signInLabel.Text = "Login Success!";
					await App.Current.MainPage.Navigation.PopModalAsync();
					App.Current.MainPage = new AppShell();
				}
			} catch (Exception e) {
				// Do something with sign-in failure.
				signInLabel.Text = "Login Failed... Please try again.";
			}
		}

		private async void SignInClicked (object sender, EventArgs e) {
			await SignIn();
		}




	}
}