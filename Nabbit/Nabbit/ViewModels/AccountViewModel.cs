using Microsoft.AppCenter.Auth;
using Nabbit.Models;
using Nabbit.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.ViewModels {
	public class AccountViewModel {
		public User UserInfo { get; set; }
		public string SchoolName { get; set; }

		public AccountViewModel() {
			UserInfo = LocalGlobals.User;
			SchoolName = LocalGlobals.School.Name;
		}
	}
}
