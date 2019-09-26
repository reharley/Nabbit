using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Nabbit.Models {
	public class Menu {
		public Menu() {
			ProductCategoryIds = new List<Guid>();
		}

		public Guid MenuId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public byte Rank { get; set; }
		public Hours Hours { get; set; }

		public List<Guid> ProductCategoryIds { get; set; }
	}
}
