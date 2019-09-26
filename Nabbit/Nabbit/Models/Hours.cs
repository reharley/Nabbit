using System;
using System.Collections.Generic;

namespace Nabbit.Models {
	public class Hours {
		public List<TimeSpan?> Opening { get; set; }
		public List<TimeSpan?> Closing { get; set; }

		public Hours () {

		}

		public static List<string> BuildDays () {
			var days = new List<string>();
			for (int i = 0; i < 7; i++) {
				if (i == (int)DayOfWeek.Monday)
					days.Add("Monday");
				else if(i == (int)DayOfWeek.Tuesday)
					days.Add("Tuesday");
				else if (i == (int)DayOfWeek.Wednesday)
					days.Add("Wednesday");
				else if (i == (int)DayOfWeek.Thursday)
					days.Add("Thursday");
				else if (i == (int)DayOfWeek.Friday)
					days.Add("Friday");
				else if (i == (int)DayOfWeek.Saturday)
					days.Add("Saturday");
				else if (i == (int)DayOfWeek.Sunday)
					days.Add("Sunday");
			}

			return days;
		}

		public void BuildHours () {
			Opening = new List<TimeSpan?>();
			Closing = new List<TimeSpan?>();
			for (int i = 0; i < 7; i++) {
				Opening.Add(new TimeSpan());
				Closing.Add(new TimeSpan());
			}
		}
	}
}
