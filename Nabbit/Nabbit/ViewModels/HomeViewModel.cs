﻿using Nabbit.Models;
using Nabbit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nabbit.ViewModels {
    public class HomeViewModel : BaseViewModel {
        List<ProductCategoryProducts> _pcProducts;
        public List<ProductCategoryProducts> pcProducts {
            get {
                return _pcProducts;
            }
            set {
                SetProperty(ref _pcProducts, value);
            }
        }

        public HomeViewModel () {
			BuildCategoryProducts();
        }

        public void BuildCategoryProducts () {
            if (LocalGlobals.Restaurant != null) {
                foreach (var menu in LocalGlobals.Restaurant.Menus) {
                    foreach (var category in LocalGlobals.Restaurant.ProductCategories
                                                    .Where(pc => menu.ProductCategoryIds.Contains(pc.ProductCategoryId))
                                                    .OrderBy(pc => pc.Rank)) {
                        var pcProduct = new ProductCategoryProducts {
                            CategoryName = category.Name,
                            MenuName = menu.Name,
                            MenuDescription = menu.Description,
                            Products = LocalGlobals.Restaurant.Products.Where(p => category.ProductIds.Contains(p.ProductId)).ToList()
                        };

                        pcProducts.Add(pcProduct);
                    }
                }
            }
        }
    }
}