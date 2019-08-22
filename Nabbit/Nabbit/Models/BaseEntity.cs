using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Nabbit.Models {
	public class BaseEntity {
		public DateTimeOffset CreatedAt { get; set; }

		public BaseEntity() {
			this.CreatedAt = DateTime.Now;
		}
	}
}
