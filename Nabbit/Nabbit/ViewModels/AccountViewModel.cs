using Microsoft.AppCenter.Auth;
using Nabbit.Models;
using Nabbit.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.ViewModels {
	public class AccountViewModel {
		public AccountViewModel() {
			UserInfo = LocalGlobals.User;
		}
		public User UserInfo { get; set; }
	}
}
