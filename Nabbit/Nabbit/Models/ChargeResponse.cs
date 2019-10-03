using System;
namespace Nabbit.Models {
	public class ChargeResponse {
		public string Message { get; set; }
		public int Status { get; set; }

		public ChargeResponse(string message, int status) {
			Message = message;
			Status = status;
		}
	}
}
