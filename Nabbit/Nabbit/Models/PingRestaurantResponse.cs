using System;
namespace Nabbit.Models {
	public class PingRestaurantResponse {
		public PingRestaurantResponse (bool isDevice = false, bool updateRestaurant = false) {
			IsDevice = isDevice;
			UpdateRestaurant = updateRestaurant;
		}

		public bool IsDevice { get; set; }
		public bool UpdateRestaurant { get; set; }
	}
}
