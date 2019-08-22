using Nabbit.Models;
using Nabbit.Services;
using Nabbit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nabbit.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OrderItemEditPage : ContentPage {
		public OrderItemEditViewModel viewModel;
		bool inCart;

		public OrderItemEditPage(Guid id, bool cartId = false) {
			InitializeComponent();
			BindingContext = viewModel = new OrderItemEditViewModel(id, cartId);
			inCart = cartId;
			if (cartId) {
				ChangeActionButtons();
				SumAddonPrices();
				RegisterSelectedItems();
			}
			AdjustGroupListHeight();
		}

		private async void CancelPressed(object sender, EventArgs e) {
			if (inCart) {
				var orderItem = Cart.OrderItems.First(o => o.OrderItemId == viewModel.OrderItem.OrderItemId);
				Cart.OrderItems.Remove(orderItem);
			}

			await Navigation.PopModalAsync();
		}

		private async void AddCartPressed(object sender, EventArgs e) {
			var addons = GetSelectedAddons();
			viewModel.OrderItem.Addons = new List<Addon>(addons);

			if (inCart) {
				var orderItem = Cart.OrderItems.First(o => o.OrderItemId == viewModel.OrderItem.OrderItemId);
				Cart.OrderItems.Remove(orderItem);
			}

			Cart.OrderItems.Add(viewModel.OrderItem);
			await Navigation.PopModalAsync();
		}

		void AdjustGroupListHeight() {
			for (int i = 0; i < groupList.Children.Count; i++) {
				var stackLayout = groupList.Children[i] as StackLayout;
				var collectionView = stackLayout.Children[1] as CollectionView;

				int addonCount = collectionView.ItemsSource.OfType<Addon>().Count();
				int addonHeight = 60, space = 10;
				
				collectionView.HeightRequest = (addonCount * addonHeight) + (addonCount * space);
			}
		}

		void SumAddonPrices(object sender, EventArgs e) {
			SumAddonPrices();
		}

		void SumAddonPrices() {
			var addons = GetSelectedAddons();

			decimal sum = viewModel.OrderItem.Product.Price;
			foreach (var addon in addons)
				sum += addon.Price;
			viewModel.Price = sum;
		}

		List<Addon> GetSelectedAddons() {
			var addons = new List<Addon>();
			for (int i = 0; i < groupList.Children.Count; i++) {
				var stackLayout = groupList.Children[i] as StackLayout;
				var collectionView = stackLayout.Children[1] as CollectionView;

				if (viewModel.AddonGroups[i].SelectionMode == SelectionMode.Multiple)
					addons.AddRange(collectionView.SelectedItems.OfType<Addon>());
				else if (viewModel.AddonGroups[i].SelectionMode == SelectionMode.Single)
					addons.Add(collectionView.SelectedItem as Addon);
			}

			return addons;
		}

		/// <summary>
		/// Tells the CollectionView groupList what items are already selected by the user previously
		/// </summary>
		void RegisterSelectedItems() {
			var addonIds = viewModel.OrderItem.Addons.Select(a => a.AddonId);
			for (int i = 0; i < groupList.Children.Count; i++) {
				var stackLayout = groupList.Children[i] as StackLayout;
				var collectionView = stackLayout.Children[1] as CollectionView;

				var addons = viewModel.AddonGroups[i].Addons.Where(a => addonIds.Contains(a.AddonId)).ToList<object>();
				if (viewModel.AddonGroups[i].SelectionMode == SelectionMode.Multiple)
					collectionView.SelectedItems = addons;
				else if (viewModel.AddonGroups[i].SelectionMode == SelectionMode.Single)
					collectionView.SelectedItem = addons[0] as object;
			}
		}

		void ChangeActionButtons() {
			cancelRemoveButton.BackgroundColor = Color.Red;
			cancelRemoveButton.Text = "Delete";

			addSaveButton.Text = "Save Changes";
		}
	}
}