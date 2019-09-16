using System;
using Nabbit.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(Nabbit.Droid.Services.BaseUrl_Android))]
namespace Nabbit.Droid.Services {
	public class BaseUrl_Android : IBaseUrl {
		public string Get() {
			return "file:///android_asset";
		}
	}
}
