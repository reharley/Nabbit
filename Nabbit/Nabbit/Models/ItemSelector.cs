using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.Models {
	public class ItemSelector<T> {
		public T Item { get; set; }
		public bool Selected { get; set; }
	}
}
