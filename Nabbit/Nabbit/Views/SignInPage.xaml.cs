using Microsoft.AppCenter.Auth;
using Microsoft.Identity.Client;
using Nabbit.Models;
using Nabbit.Services;
using Nabbit.Services.LogOn;
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
		protected readonly IAuthenticationService authenticationService;
		BaseViewModel viewModel;

		public SignInPage () {
			InitializeComponent();
			BindingContext = viewModel = new BaseViewModel();
			authenticationService = DependencyService.Get<IAuthenticationService>();
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

				await App.Current.MainPage.Navigation.PopModalAsync();
			} catch (Exception e) {
				// Do something with sign-in failure.
				signInLabel.Text = "Login Failed... Please try again.";
			}

			viewModel.IsBusy = false;
		}

		/*
		private async void SignInClicked (object sender, EventArgs e) {
			await SignIn();
		}
		*/

		private async void CancelClicked (object sender, EventArgs e) {
			await Navigation.PopModalAsync();
		}

		async void SignInClicked (object sender, EventArgs e) {
			try {
				if (LocalGlobals.User.LoggedIn == false) {
					var userContext = await authenticationService.SignInAsync();
					await UpdateUserInfo(userContext);
				} else {
					var userContext = await authenticationService.SignOutAsync();
					await UpdateUserInfo(userContext);
				}
			} catch (Exception ex) {
				// Checking the exception message 
				// should ONLY be done for B2C
				// reset and not any other error.
				if (ex.Message.Contains("AADB2C90118"))
					OnPasswordReset();
				// Alert if any exception excluding user cancelling sign-in dialog
				//else if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
				//	await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
			}
		}

		async void OnPasswordReset () {
			try {
				var userContext = await authenticationService.ResetPasswordAsync();
				await UpdateUserInfo(userContext);
			} catch (Exception ex) {
				// Alert if any exception excludig user cancelling sign-in dialog
				if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
					await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
			}
		}

		public async Task UpdateUserInfo (UserContext userContext) {
			LocalGlobals.User.FirstName = userContext.Name;
			LocalGlobals.User.LastName = userContext.FamilyName;
			LocalGlobals.User.Email = userContext.EmailAddress;
			LocalGlobals.User.UserId = new Guid(userContext.UserIdentifier);
			await LocalGlobals.GetUser();
		}
	}
}