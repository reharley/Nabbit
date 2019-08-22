using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.Models {
	public class ProductCategoryProducts {
		public string CategoryName { get; set; }
		public string MenuName { get; set; }
		public string MenuDescription { get; set; }
		public List<Product> Products { get; set; }
	}
}
