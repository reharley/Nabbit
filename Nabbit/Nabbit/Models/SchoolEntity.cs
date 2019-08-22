using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.Models {
	public class SchoolEntity : TableEntity {
		public static string PartionKeyLabel = "school";
		public SchoolEntity() { }
		public SchoolEntity(string schoolId) {
			this.PartitionKey = PartionKeyLabel;
			this.RowKey = schoolId;
		}

		public SchoolEntity(string schoolId, string json) {
			this.PartitionKey = PartionKeyLabel;
			this.RowKey = schoolId;

			JSON = json;
		}

		public string JSON { get; set; }
	}
}
