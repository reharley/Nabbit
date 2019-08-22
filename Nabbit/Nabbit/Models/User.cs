using Microsoft.AppCenter.Auth;
using Newtonsoft.Json;
using Plugin.SecureStorage;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace Nabbit.Models {
	public class User {
		public User() {
			LoggedIn = false;
		}

		public Guid UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string School { get; set; }
		public bool LoggedIn { get; set; }
	}
}
