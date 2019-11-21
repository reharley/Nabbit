namespace Nabbit.Services.LogOn {
	public class UserContext {
		public string Name { get; internal set; }
		public string UserIdentifier { get; internal set; }
		public bool IsLoggedOn { get; internal set; }
		public string GivenName { get; internal set; }
		public string FamilyName { get; internal set; }
		public string EmailAddress { get; internal set; }
		public string AccessToken { get; internal set; }
	}
}