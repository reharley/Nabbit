using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Nabbit.Models;
using Nabbit.Services;
using Newtonsoft.Json;

namespace Nabbit.Console {
	class Program {
#if DEBUG
		private const string getSchoolsUrl = "https://nabbitdev.azurewebsites.net/api/userId/{userId}?code=0y3yfBDdRd4iZBAgUmQzuxZAma1uvtRUCZ3iAi9TvY7Pbp7CO35Rvw==";
		private const string getRestaurantsUrl = "https://nabbitdev.azurewebsites.net/api/userId/{userId}/schoolId/{schoolId}?code=IPJvBINpgLWBtZMltTWzp1gKhp7K3UYr8GDZH8XlSoRMgyiFhq9Okg==";
#else
		private const string getSchoolsUrl = "https://nabbit.azurewebsites.net/api/userId/{userId}?code=ztgeYLZ/QNjKE26BoC9fb/R6PpvL0dNAlzH3r2dC0QUtwtaKs2tWsg==";
		private const string getRestaurantsUrl = "https://nabbit.azurewebsites.net/api/userId/{userId}/schoolId/{schoolId}?code=ZX45t3u8uyrT24p6bbBFXhepqeQ7KoKGN9N/lbl1p8vakNTHsgw/ng==";
#endif


		static void Main (string[] args) {
			try {
				Init().Wait();

				System.Console.Beep();
				System.Console.Beep();
				System.Console.Beep();
				bool serviceUp = false;
				var lastPing = DateTime.Now.TimeOfDay;
				while (true) {

					LocalGlobals.GetRestaurant().Wait();

					var now = DateTime.Now;
					var restPing = LocalGlobals.Restaurant.LastPing;
					var updateDiff = now - restPing;
					System.Console.WriteLine();
					System.Console.WriteLine($"Ping: {now.ToString("hh:mm:ss tt")}");
					System.Console.WriteLine($"Last Rest Ping: {restPing.ToString("hh:mm:ss tt")}");
					System.Console.WriteLine($"Ping diff: {updateDiff.ToString("h'h 'm'm 's's'")}");
					serviceUp = true;
					if (updateDiff > new TimeSpan(0, 1, 30)) {
						System.Console.Beep();
						System.Console.Beep();
						System.Console.Beep();
						System.Console.WriteLine("Service is Down!");
						serviceUp = false;
					}
					lastPing = DateTime.Now.TimeOfDay;

					if (serviceUp)
						Thread.Sleep(80000);
					else
						Thread.Sleep(1000);
				}
			} catch (Exception e) {
				System.Console.WriteLine(e.Message);
				System.Console.WriteLine("Exited");
			}
		}

		static async Task Init () {
			await PullObjects();
		}

		static async Task CheckConnection () {
			await LocalGlobals.GetRestaurant();
		}

		static async Task PullObjects () {
			using (var client = new HttpClient()) {
				var url = getSchoolsUrl.Replace("{userId}", "none");
				string result = "";

				using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
					result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
				}
				var schoolEntity = JsonConvert.DeserializeObject<List<SchoolEntity>>(result)[0];
				var school = LocalGlobals.School = JsonConvert.DeserializeObject<School>(schoolEntity.JSON);

				url = getRestaurantsUrl.Replace("{userId}", "none").Replace("{schoolId}", school.SchoolId.ToString());
				using (var httpResponse = await client.GetAsync(url).ConfigureAwait(false)) {
					result = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
				}
				var restaurantEntity = JsonConvert.DeserializeObject<List<RestaurantEntity>>(result)[0];
				LocalGlobals.Restaurant = JsonConvert.DeserializeObject<Restaurant>(restaurantEntity.JSON);
			}
		}

	}
}
