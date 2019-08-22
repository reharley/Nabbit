using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Nabbit.Models {
	public partial class Menu {
		public Menu() {
			ProductCategoryIds = new HashSet<Guid>();
		}
		public Guid MenuId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public byte Rank { get; set; }
		public string OpeningHour { get; set; }
		public string ClosingHour { get; set; }

		public ICollection<Guid> ProductCategoryIds { get; set; }
	}
}
