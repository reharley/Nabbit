using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Nabbit.Models {
	public partial class ProductCategory {
		public ProductCategory() {
			ProductIds = new List<Guid>();
			AddonGroupIds = new List<Guid>();
			Enabled = true;
		}

		public Guid ProductCategoryId { get; set; }
		public string Name { get; set; }
		public int Rank { get; set; }
		public bool Enabled { get; set; }
		
		public ICollection<Guid> ProductIds { get; set; }
		public ICollection<Guid> AddonGroupIds { get; set; }
	}
}
