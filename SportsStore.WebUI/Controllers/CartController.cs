using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        IProductRepository repository;
        public CartController(IProductRepository repoParam)
        {
            repository = repoParam;
        }

        public RedirectToRouteResult AddToCart(int productID, string returnUrl)
        {
            // ta linia pobiera do zmiennej product wybrany produkt do dodania do koszyka
            Product product = repository.Products.FirstOrDefault(p => p.ProductID == productID);

            //ta linia sprawdza czy produkt jest w bazie danych
            if(product != null)
                GetCart().AddItem(product, 1);
            return RedirectToAction("Index", new { returnUrl });
        }

        public RedirectToRouteResult RemoveFromCart(int productID, string returnUrl)
        {
            Product product = repository.Products.FirstOrDefault(p => p.ProductID == productID);
            if (product != null)
                GetCart().RemoveLine(product);
            return RedirectToAction("Index", new { returnUrl });
        }


        private Cart GetCart()
        {
            Cart cart = (Cart)Session["Cart"];
            if (cart == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }
    }
}