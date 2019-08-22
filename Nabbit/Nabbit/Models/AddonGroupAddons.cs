using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Nabbit.Models {
	public class AddonGroupAddons {
		public Guid AddonGroupId { get; set; }
		public string Name { get; set; }
		public string SelectType { get; set; }
		public SelectionMode SelectionMode { get; set; }
		public Addon DefaultAddon { get; set; }
		public ICollection<Addon> DefaultAddons { get; set; }

		public List<Addon> Addons { get; set; }
	}
}
