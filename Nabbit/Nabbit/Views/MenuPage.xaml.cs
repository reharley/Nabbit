using Nabbit.Models;
using Nabbit.Services;
using Nabbit.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nabbit.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MenuPage : ContentPage {
		public HomeViewModel viewModel;
		bool gridDefined = false;

		public MenuPage() {
			InitializeComponent();
			BindingContext = viewModel = new HomeViewModel();
			AdjustGroupListHeight();
		}

		void AdjustGroupListHeight () {
			for (int i = 0; i < groupList.Children.Count; i++) {
				var stackLayout = groupList.Children[i] as StackLayout;
				var collectionView = stackLayout.Children[1] as CollectionView;

				int productCount = collectionView.ItemsSource.OfType<Product>().Count();
				int labelHeight = 30, space = 10;
				if (Device.RuntimePlatform == Device.Android)
					labelHeight = 55;

				collectionView.HeightRequest = (productCount * labelHeight) + (productCount * space);
			}
		}
	}
}