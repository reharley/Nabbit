using Nabbit.Models;
using Nabbit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nabbit.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ThankYouPage : ContentPage {
		public ThankYouPage() {
			InitializeComponent();
			Cart.OrderItems.Clear();
		}

		private async void ReturnClicked(object sender, EventArgs e) {
			var pages = Navigation.ModalStack.Count;
			for(int i = 0; i < pages; i++)
				Navigation.PopModalAsync();
		}
	}
}