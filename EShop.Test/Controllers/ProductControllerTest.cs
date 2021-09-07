using EShop.Domain.DomainModels;
using EShop.Domain.DTO;
using EShop.Services.Interface;
using EShop.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace EShop.Test
{
    public class ProductControllerTest
    {
        private Mock<IProductService> _productService;
        private Mock<IUserService> _userService;
        private ProductsController productsController;

        public ProductControllerTest() 
        {
            this._productService = new Mock<IProductService>();
            this._userService = new Mock<IUserService>();
            this.productsController = new ProductsController(_productService.Object,_userService.Object);
        }

        [Fact]
        public void IndexTest_ReturnsViewWithListOfProducts()
        { 
            // Arange
            var listProducts = new List<Product>()
            {
                new Product
                {
                    ProductName = "Kafe",
                    ProductImage = "https://image.shutterstock.com/image-photo/cappuccino-milk-coffee-bean-on-260nw-791935171.jpg",
                    ProductDescription = "Malo makiato so izobilni vkusovi na zrno tursko kafe",
                    ProductPrice = 120,
                    Rating = 10
                },
                 new Product
                {
                    ProductName = "Cokolada",
                    ProductImage = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f2/Chocolate.jpg/250px-Chocolate.jpg",
                    ProductDescription = "Cokolatce za so kafe",
                    ProductPrice = 120,
                    Rating = 10
                },
                  new Product
                {
                    ProductName = "Krastavica",
                    ProductImage = "https://agrohemija.com.mk/wp-content/uploads/2018/05/Krastavica-dolga.jpg",
                    ProductDescription = "Krastavicka....!!",
                    ProductPrice = 120,
                    Rating = 10
                }
            };

            // Setiranje na _productService mock
            _productService.Setup(m => m.GetAllProducts()).Returns(listProducts);

            //Act
            var result = productsController.Index();

            //Assert

            var isViewReuslt = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ICollection<Product>>(isViewReuslt.ViewData.Model);
            Assert.Equal(3, model.Count);
        }
        
        [Fact]
        public void AddProductToCartTest_ReturnsViewWithAddToShopingCartDtoModel() 
        {
            //Arange
            Guid id = Guid.NewGuid();
            Product tmp = new Product
            {
                Id = id,
                ProductName = "Kafe",
                ProductImage = "https://image.shutterstock.com/image-photo/cappuccino-milk-coffee-bean-on-260nw-791935171.jpg",
                ProductDescription = "Malo makiato so izobilni vkusovi na zrno tursko kafe",
                ProductPrice = 120,
                Rating = 10
            };

            AddToShoppingCardDto dto = new AddToShoppingCardDto()
            {
                ProductId = id,
                Quantity = 3
            };

            _productService.Setup(m => m.GetShoppingCartInfo(id)).Returns(dto);

            //ACT
            var result = productsController.AddProductToCard(id);

            //Assert
            var isViewReuslt = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AddToShoppingCardDto>(isViewReuslt.ViewData.Model);
            Assert.Equal(model.ProductId, id);
        }

        [Fact]
        public void AddProductToCartTest_PostActionSuccess() 
        {
            //Arange
            Guid id = Guid.NewGuid();
            Product tmp = new Product
            {
                Id = id,
                ProductName = "Kafe",
                ProductImage = "https://image.shutterstock.com/image-photo/cappuccino-milk-coffee-bean-on-260nw-791935171.jpg",
                ProductDescription = "Malo makiato so izobilni vkusovi na zrno tursko kafe",
                ProductPrice = 120,
                Rating = 10
            };

            AddToShoppingCardDto dto = new AddToShoppingCardDto()
            {
                ProductId = id,
                Quantity = 3
            };

            var userId = Guid.NewGuid().ToString();

            _productService.Setup(m => m.AddToShoppingCart(dto, userId)).Returns(true);
            _userService.Setup(m => m.GetUserId()).Returns(userId);

            //ACT
            var result = productsController.AddProductToCard(dto);

            //Assert
            var isRedirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", isRedirectToActionResult.ActionName);
            Assert.Equal("Products", isRedirectToActionResult.ControllerName);

        }

        [Fact]

        public void AddProductToCartTest_PostActionFailed()
        {
            //Arange
            Guid id = Guid.NewGuid();
            Product tmp = new Product
            {
                Id = id,
                ProductName = "Kafe",
                ProductImage = "https://image.shutterstock.com/image-photo/cappuccino-milk-coffee-bean-on-260nw-791935171.jpg",
                ProductDescription = "Malo makiato so izobilni vkusovi na zrno tursko kafe",
                ProductPrice = 120,
                Rating = 10
            };

            AddToShoppingCardDto dto = new AddToShoppingCardDto()
            {
                ProductId = id,
                Quantity = 3
            };

            var userId = Guid.NewGuid().ToString();

            _productService.Setup(m => m.AddToShoppingCart(dto, userId)).Returns(false);
            _userService.Setup(m => m.GetUserId()).Returns(userId);

            //ACT
            var result = productsController.AddProductToCard(dto);

            //Assert

            var isViewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AddToShoppingCardDto>(isViewResult.ViewData.Model);

        }
        //06.09.2021
        [Fact]
        public void CreateTestPost_CreateViewFail()
        {
            Guid id = Guid.NewGuid();
            Product tmp = new Product
            {
                Id = id,
                ProductName = "TestName",
                ProductDescription = "TestDescription",
                ProductImage = "SOMEURL",
                ProductPrice = 200,
                Rating = 5
            };

            productsController.ModelState.AddModelError("Description", "This Field Is Required");

            var result = productsController.Create(tmp);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(tmp, viewResult.ViewData.Model);
            _productService.Verify(z => z.CreateNewProduct(tmp), Times.Never());
        }

        [Fact]
        public void CreateTestPost_RedirectToIndex()
        {
            Guid id = Guid.NewGuid();
            Product tmp = new Product
            {
                Id = id,
                ProductName = "TestName",
                ProductDescription = "TestDescription",
                ProductImage = "SOMEURL",
                ProductPrice = 200,
                Rating = 5
            };

            _productService.Setup(z => z.CreateNewProduct(tmp));

            var result = productsController.Create(tmp);

            _productService.Verify(z => z.CreateNewProduct(tmp));
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", viewResult.ActionName);
        }

        [Fact]
        public void DetailsTest_NoID()
        {
            var result = productsController.Details(null);

            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DetailsTest_NotExist()
        {
            Guid id = Guid.NewGuid();
            _productService.Setup(z => z.GetDetailsForProduct(id));

            var result = productsController.Details(id);

            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DetailsTest_Exist()
        {
            Guid id = Guid.NewGuid();
            Product tmp = new Product
            {
                Id = id,
                ProductName = "TestName",
                ProductDescription = "TestDescription",
                ProductImage = "SOMEURL",
                ProductPrice = 200,
                Rating = 5
            };

            _productService.Setup(z => z.GetDetailsForProduct(id)).Returns(tmp);

            var result = productsController.Details(id);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(tmp, viewResult.ViewData.Model);
        }
    }
}
