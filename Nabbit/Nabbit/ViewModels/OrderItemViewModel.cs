using Nabbit.Models;
using Nabbit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Nabbit.ViewModels {
	public class OrderItemEditViewModel : BaseViewModel {
		public bool InCart { get; set; }
		OrderItem orderItem;
		public OrderItem OrderItem {
			get { return orderItem; }
			set { SetProperty(ref orderItem, value); }
		}

		decimal price;
		public decimal Price {
			get {
				return price;
			}
			set {
				SetProperty(ref price, value);
			}
		}

		public List<AddonGroupAddons> AddonGroups { get; set; }

		public OrderItemEditViewModel(Guid id) {
			InCart = false;
			var item = Cart.OrderItems.FirstOrDefault(x => x.OrderItemId == id);

			if (item != null) {
				OrderItem = item;
				InCart = true;
			} else {
				OrderItem = new OrderItem() {
					OrderItemId = Guid.NewGuid(),
					Product = LocalGlobals.Restaurant.Products.FirstOrDefault(p => p.ProductId == id),
					Quantity = 1,
					Instructions = ""
				};
			}

			BuildViewModel();
		}

		public OrderItemEditViewModel(OrderItem orderItem) {
			OrderItem = orderItem;

			BuildViewModel();
		}

		void BuildViewModel() {
			Price = OrderItem.Product.Price;
			AddonGroups = AddonGroupAddons.BuildAddonGroups(OrderItem);
		}
	}
}
