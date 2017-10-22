using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using System.Linq;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //przygotowanie
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Cart cart = new Cart();
            //dzialanie
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);
            CartLine[] results = cart.Lines.ToArray();
            //asercja
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void Canan_Add_Quantity_For_Existing_Lines()
        {
            //przygotowanie
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Cart cart = new Cart();
            //działanie
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);
            cart.AddItem(p1, 10);
            CartLine[] results = cart.Lines.OrderBy(c => c.Product.ProductID).ToArray();
            //asercja
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Lines()
        {
            //przygotowanie - utworzenie produktow
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };
            //przygotowanie  - utworzenie koszyka
            Cart cart = new Cart();
            //przygotowanie - dodanie produktów do koszyka
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 3);
            cart.AddItem(p3, 5);
            cart.AddItem(p2, 1);
            //działanie - usuniecie produkty p2
            cart.RemoveLine(p2);
            //asercja
            Assert.IsNull(cart.Lines.FirstOrDefault(p => p.Product.ProductID == 2));
            Assert.AreEqual(cart.Lines.Count(), 2);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            // przygotowanie — tworzenie produktów testowych
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };
            // przygotowanie — utworzenie nowego koszyka
            Cart cart = new Cart();
            // działanie
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);
            cart.AddItem(p1, 3);
            decimal result = cart.ComputeTotalValue();
            //asercja 
            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            // przygotowanie — tworzenie produktów testowych
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };
            // przygotowanie — utworzenie nowego koszyka
            Cart target = new Cart();
            // przygotowanie — dodanie kilku produktów do koszyka
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            // działanie — czyszczenie koszyka
            target.Clear();
            // asercje
            Assert.AreEqual(target.Lines.Count(), 0);
        }

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            // przygotowanie — tworzenie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                    new Product {ProductID = 1, Name = "P1", Category = "Jab"},
                    }.AsQueryable());
            // przygotowanie — utworzenie koszyka
            Cart cart = new Cart();
            // przygotowanie — utworzenie kontrolera
            CartController target = new CartController(mock.Object, null);
            // działanie — dodanie produktu do koszyka
            target.AddToCart(cart, 1, null);
            //asercja
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            // przygotowanie — tworzenie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
        new Product {ProductID = 1, Name = "P1", Category = "Jabłka"},
        }.AsQueryable());
            // przygotowanie — utworzenie koszyka
            Cart cart = new Cart();
            // przygotowanie — utworzenie kontrolera
            CartController target = new CartController(mock.Object, null);
            // działanie — dodanie produktu do koszyka
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");
            // asercje
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            // przygotowanie — tworzenie imitacji procesora zamówień
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            // przygotowanie — tworzenie pustego koszyka
            Cart cart = new Cart();
            // przygotowanie — tworzenie danych do wysyłki
            ShippingDetails shippingDetails = new ShippingDetails();
            // przygotowanie — tworzenie egzemplarza kontrolera
            CartController target = new CartController(null, mock.Object);
            // działanie
            ViewResult result = target.Checkout(cart, shippingDetails);
            // asercje — sprawdzenie, czy zamówienie zostało przekazane do procesora
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()),
            Times.Never());
            // asercje — sprawdzenie, czy metoda zwraca domyślny widok
            Assert.AreEqual("", result.ViewName);
            // asercje — sprawdzenie, czy przekazujemy prawidłowy model do widoku
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }
    }
}
