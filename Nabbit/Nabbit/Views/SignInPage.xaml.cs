using Microsoft.AppCenter.Auth;
using Nabbit.Services;
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
	public partial class SignInPage : ContentPage {
		public SignInPage() {
			InitializeComponent();
		}

		async Task SignIn() {
			try {
				// Sign-in succeeded.
				UserInformation userInfo = await Auth.SignInAsync();
				CrossSecureStorage.Current.SetValue("UserToken", userInfo.IdToken);
				await LocalGlobals.GetUser();
				if (LocalGlobals.User.CustomerId == Guid.Empty) {
					await CreateCustomer();
					await LocalGlobals.PostUser(LocalGlobals.User);
				}

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

		async Task CreateCustomer () {
			var baseAddress = new Uri("https://apiprod.fattlabs.com/");

			using (var httpClient = new HttpClient { BaseAddress = baseAddress }) {
				var empty = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJtZXJjaGFudCI6IjA1ZDZkZDM4LTdlZjQtNDdlZi1iYTM1LWVlY2I1ZTE0MTkwMSIsImdvZFVzZXIiOmZhbHNlLCJzdWIiOiI0N2RkNTY5Yi1hZTVmLTQ2M2YtYmNiZC02NWUyOGIzMWRiNTYiLCJpc3MiOiJodHRwOi8vYXBpcHJvZC5mYXR0bGFicy5jb20vdGVhbS9hcGlrZXkiLCJpYXQiOjE1Njg3NTU1MDUsImV4cCI6NDcyMjM1NTUwNSwibmJmIjoxNTY4NzU1NTA1LCJqdGkiOiJyRXVyRFA2amdYUExKRk54In0.pm7ut5ywZMfuL23Cc0ZRyqAL_IBDh_DZmCxF7iAu_lQ";
				httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", $"Bearer {empty}");

				httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");

				var user = LocalGlobals.User;
				using (var content = new StringContent("{  \"email\": \"" + user.Email + "\",\"firstname\": \"" + user.FirstName + "\",  \"lastname\": \"" + user.LastName + "\"}",
															System.Text.Encoding.Default, "application/json")) {
					using (var response = await httpClient.PostAsync("customer", content)) {
						string responseData = await response.Content.ReadAsStringAsync();
						var responseObj = JObject.Parse(responseData);
						if (responseObj["id"] != null)
							LocalGlobals.User.CustomerId = Guid.Parse((string)(responseObj["id"]));
					}
				}
			}
		}
	}
}