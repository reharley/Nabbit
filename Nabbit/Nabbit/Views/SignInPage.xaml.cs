using Microsoft.AppCenter.Auth;
using Nabbit.Models;
using Nabbit.Services;
using Nabbit.ViewModels;
using Newtonsoft.Json.Linq;
using Plugin.SecureStorage;
using Stripe;
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
		BaseViewModel viewModel;
		public SignInPage () {
			InitializeComponent();
			BindingContext = viewModel = new BaseViewModel();
		}

		async Task SignIn () {
			viewModel.IsBusy = true;
			try {
				// Sign-in succeeded.
				UserInformation userInfo = await Auth.SignInAsync();
				var tokenHandler = new JwtSecurityTokenHandler();
				var jwToken = tokenHandler.ReadJwtToken(userInfo.IdToken);

				var firstNameClaim = jwToken.Claims.FirstOrDefault(t => t.Type == "given_name");
				var lastNameClaim = jwToken.Claims.FirstOrDefault(t => t.Type == "family_name");
				var oidClaim = jwToken.Claims.FirstOrDefault(t => t.Type == "oid");
				var emailClaim = jwToken.Claims.FirstOrDefault(t => t.Type == "emails");

				string firstName = "";
				string lastName = "";

				if (firstNameClaim != null)
					firstName = firstNameClaim.Value;
				if (lastNameClaim != null)
					lastName = lastNameClaim.Value;

				LocalGlobals.User.FirstName = firstName;
				LocalGlobals.User.LastName = lastName;
				LocalGlobals.User.UserId = new Guid(jwToken.Claims.FirstOrDefault(t => t.Type == "oid").Value);
				LocalGlobals.User.Email = jwToken.Claims.FirstOrDefault(t => t.Type == "emails").Value;

				await LocalGlobals.GetUser();

				signInLabel.Text = "Login Success!";
				await App.Current.MainPage.Navigation.PopModalAsync();
			} catch (Exception e) {
				// Do something with sign-in failure.
				signInLabel.Text = "Login Failed... Please try again.";
			}

			viewModel.IsBusy = false;
		}

		private async void SignInClicked (object sender, EventArgs e) {
			await SignIn();
		}


		private async void CancelClicked (object sender, EventArgs e) {
			await Navigation.PopModalAsync();
		}
	}
}