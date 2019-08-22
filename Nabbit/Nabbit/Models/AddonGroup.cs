using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Nabbit.Models {
	public partial class AddonGroup {
		public AddonGroup() {
			AddonIds = new HashSet<Guid>();
		}

		public Guid AddonGroupId { get; set; }
		public string Name { get; set; }
		public string SelectType { get; set; }

		public ICollection<Guid> AddonIds { get; set; }
	}

	/// <summary>
	/// Keeps track of all the types that can be used with forms for AddonGroup.SelectType.
	/// </summary>
	public static class SelectTypes {
		public const string Radio = "Radio Buttons";
		public const string Check = "Check Boxes";
		public const string Drop = "Drop Down";
	}
}
