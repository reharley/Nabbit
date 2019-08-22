using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NabbitManager.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OrderMasterPage : ContentPage {
		public OrderMasterPage() {
			InitializeComponent();

			List<Button> Buttons = new List<Button>();
			List<string> titles = new List<string>() {
				"All Orders",
				"Live Orders"
			};

			var uris = new List<string>() {
			"allOrders","liveOrders"
			};

			navButtons.Children.Clear();
			for (int i = 0; i < titles.Count; i++) {
				var button = new Button() {
					Text = titles[i]
				};
				var uri = uris[i];
				button.Pressed += async (sender, args) => {
					await Shell.Current.GoToAsync(uri);
				};
				navButtons.Children.Add(button);
			}
		}
	}
}