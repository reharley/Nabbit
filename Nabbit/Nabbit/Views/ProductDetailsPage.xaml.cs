using Nabbit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nabbit.Views
{
	[QueryProperty("Entry", "name")]
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProductDetailsPage : ContentPage {
		public ProductDetailsViewModel viewModel { get; set; }

		public string Entry {
			set {
				BindingContext = viewModel = new ProductDetailsViewModel(value);
			}
		}

		public ProductDetailsPage() {
			InitializeComponent();
			//var product = App.Restaurant.Products.FirstOrDefault(p => p.ProductId == productId);
			//productName.Text = "Studd";
		}
	}
}