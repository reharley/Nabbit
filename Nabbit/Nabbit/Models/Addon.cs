using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Nabbit.Models {
	public partial class Addon {
		public Guid AddonId { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
	}
}
