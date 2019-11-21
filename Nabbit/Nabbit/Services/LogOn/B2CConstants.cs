namespace Nabbit.Services.LogOn {
	public static class B2CConstants {
		// Azure AD B2C Coordinates
		public static string Tenant = "nabbit.onmicrosoft.com";
		public static string AzureADB2CHostname = "nabbit.b2clogin.com";

#if __IOS__
		public static string ClientID = "2297da99-c2fd-4ead-abfb-930c804e2b79";
#else
		public static string ClientID = "12023ec5-4650-478f-ae75-0421da37d23a";
#endif

		public static string PolicySignUpSignIn = "b2c_1_signUpSignIn";
		public static string PolicyEditProfile = "B2C_1_profileEditing";
		public static string PolicyResetPassword = "B2C_1_passwordReset";

		public static string[] Scopes = { "https://nabbit.onmicrosoft.com/api/user_impersonation" };

		public static string AuthorityBase = $"https://{AzureADB2CHostname}/tfp/{Tenant}/";
		public static string AuthoritySignInSignUp = $"{AuthorityBase}{PolicySignUpSignIn}";
		public static string AuthorityEditProfile = $"{AuthorityBase}{PolicyEditProfile}";
		public static string AuthorityPasswordReset = $"{AuthorityBase}{PolicyResetPassword}";
		public static string IOSKeyChainGroup = "com.microsoft.adalcache";
	}
}