using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            // przygotowanie - utworzenie obiektu mock zwracającego kilka produktów
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"}
            });
            // przygotowanie - utworzenie obiektu klasy AdminController z udziałem mocka
            AdminController controller = new AdminController(mock.Object);
            // działanie - pobranie listy produktow do zmiennej results
            Product[] result = ((IEnumerable<Product>)controller.Index().
                            ViewData.Model).ToArray();
            // asercja - porównanie właściwości Name
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }

        [TestMethod]
        public void Can_Edit_Products()
        {
            // przygotowanie - utworzenie obiektu mock zwracającego tablice produktów
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"}
            });
            // przygotowanie - utworzenie obiektu klasy AdminController z udziałem mocka
            AdminController controller = new AdminController(mock.Object);
            // działanie - wywołanie metody Edit i zapis zmodyfikowanego obiektu Product do zmiennych p1-p3
            Product p1 = controller.Edit(1).ViewData.Model as Product;
            Product p2 = controller.Edit(2).ViewData.Model as Product;
            Product p3 = controller.Edit(3).ViewData.Model as Product;
            // asercja
            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Product()
        {
            // przygotowanie — tworzenie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                    new Product {ProductID = 1, Name = "P1"},
                    new Product {ProductID = 2, Name = "P2"},
                    new Product {ProductID = 3, Name = "P3"},
                    });
            // przygotowanie — utworzenie kontrolera
            AdminController target = new AdminController(mock.Object);
            // działanie
            Product result = (Product)target.Edit(4).ViewData.Model;
            // asercje
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            // przygotowanie — tworzenie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            // przygotowanie — tworzenie kontrolera
            AdminController target = new AdminController(mock.Object);
            // przygotowanie — tworzenie produktu
            Product product = new Product { Name = "Test" };
            // działanie — próba zapisania produktu
            ActionResult result = target.Edit(product);
            // asercje — sprawdzenie, czy zostało wywołane repozytorium
            mock.Verify(m => m.SaveProduct(product));
            // asercje — sprawdzenie typu zwracanego z metody
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            // przygotowanie — tworzenie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            // przygotowanie — tworzenie kontrolera
            AdminController target = new AdminController(mock.Object);
            // przygotowanie — tworzenie produktu
            Product product = new Product { Name = "Test" };
            // przygotowanie — dodanie błędu do stanu modelu
            target.ModelState.AddModelError("error", "error");
            // działanie — próba zapisania produktu
            ActionResult result = target.Edit(product);
            // asercje — sprawdzenie, czy nie zostało wywołane repozytorium
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());
            // asercje — sprawdzenie typu zwracanego z metody
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Products()
        {
            // przygotowanie - utworzenia egzemplarza klasy Product
            Product productTodelete = new Product();
            // przygotowanie - utworzenie mocka IProductRepository oraz wstrzykniecie od konstruktora AdminController
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(new Product[]
            {
                new Product {ProductID = 1},
                productTodelete,
                new Product {ProductID = 2} 
            });
            AdminController controller = new AdminController(mock.Object);
            // działanie - usuniecie obiektu productToDelete 
            controller.Delete(productTodelete.ProductID);
            // asercja
            mock.Verify(p => p.DeleteProduct(productTodelete.ProductID));
        }
    }
}
