using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Nabbit.Models {
	public partial class BusinessHours {
		public byte DayOfWeek { get; set; }
		public string OpeningHour { get; set; }
		public string ClosingHour { get; set; }
	}
}
