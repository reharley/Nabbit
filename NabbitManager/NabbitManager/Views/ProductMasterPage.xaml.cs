using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NabbitManager.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProductMasterPage : ContentPage {
		public ProductMasterPage() {
			InitializeComponent();

			List<Button> Buttons = new List<Button>();
			List<string> titles = new List<string>() { 
				"Product Manager",
				"Product Category Manager",
				"Addon Group Manager",
				"Addon Manager",
				"Menu Manager"
			};

			var uris = new List<string>() {
			"productmanager","categorymanager","addongroupmanager",
			"addonmanager","menumanager"
			};

			navButtons.Children.Clear();
			for (int i = 0; i < titles.Count; i++) {
				var button = new Button() {
					Text = titles[i]
				};
				var uri = uris[i];
				if (i == 0) {
					button.Pressed += async (sender, args) => {
						await Navigation.PushAsync(new ProductManagerPage());
					};
				} else if (i == 1) {
					button.Pressed += async (sender, args) => {
						await Navigation.PushAsync(new CategoryManagerPage());
					};
				} else if (i == 2) {
					button.Pressed += async (sender, args) => {
						await Navigation.PushAsync(new AddonGroupManagerPage());
					};
				} else if (i == 3) {
					button.Pressed += async (sender, args) => {
						await Navigation.PushAsync(new AddonManagerPage());
					};
				} else if (i == 4) {
					button.Pressed += async (sender, args) => {
						await Navigation.PushAsync(new MenuManagerPage());
					};
				}

				navButtons.Children.Add(button);
			}
		}
	}
}