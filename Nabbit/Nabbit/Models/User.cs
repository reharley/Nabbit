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
		public User () {
			LoggedIn = false;
			FirstName = "";
			LastName = "";
			Email = "";
			CustomerId = "";
		}

		/// <summary>Internal system Id</summary>
		public Guid UserId { get; set; }
		/// <summary>School Id that the user is associated with</summary>
		public Guid SchoolId { get; set; }
		/// <summary>Id for payment methods associated with Stripe</summary>
		public string CustomerId { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }

		public bool LoggedIn { get; set; }
	}
}
