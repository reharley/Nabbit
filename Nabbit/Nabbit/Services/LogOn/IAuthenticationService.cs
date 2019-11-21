using System.Threading.Tasks;

namespace Nabbit.Services.LogOn {
	public interface IAuthenticationService {
		void SetParent (object parent);
		Task<UserContext> SignInAsync ();
		Task<UserContext> SignOutAsync ();
		Task<UserContext> EditProfileAsync ();
		Task<UserContext> ResetPasswordAsync ();
	}
}