using Microsoft.AppCenter.Auth;
using Nabbit.Models;
using Nabbit.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.ViewModels {
	public class AccountViewModel : BaseViewModel {
		User user;
		public User UserInfo {
			get {
				if (user == null)
					user = LocalGlobals.User;
				return user;
			}
			set {
				SetProperty(ref user, value);
			}
		}
		public string SchoolName { get; set; }

		public AccountViewModel () {
			user = LocalGlobals.User;
			SchoolName = LocalGlobals.School.Name;
		}
	}
}
