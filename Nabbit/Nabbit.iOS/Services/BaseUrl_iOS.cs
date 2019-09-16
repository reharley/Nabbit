using System;
using Foundation;
using Nabbit.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(Nabbit.iOS.Services.BaseUrl_iOS))]
namespace Nabbit.iOS.Services {
	public class BaseUrl_iOS : IBaseUrl {
		public string Get() {
			return NSBundle.MainBundle.BundlePath;
		}
	}
}
