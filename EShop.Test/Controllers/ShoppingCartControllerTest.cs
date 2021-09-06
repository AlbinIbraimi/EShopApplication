using EShop.Domain.DomainModels;
using EShop.Domain.DTO;
using EShop.Services.Interface;
using EShop.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EShop.Test.Controllers
{
    public class ShoppingCartControllerTest
    {
        private Mock<IShoppingCartService> _shoppingCartService;
        private ShoppingCartController _shoppingCartController;

        public ShoppingCartControllerTest()
        {
            _shoppingCartService = new Mock<IShoppingCartService>();
            _shoppingCartController = new ShoppingCartController(_shoppingCartService.Object);
        }

        [Fact]
        public void IndexTest_ReturnsShoppingCartDto()
        {
            Guid id = Guid.NewGuid();
            Guid userid = Guid.NewGuid();
            Guid shoppingcartid = Guid.NewGuid();

            var listProducts = new List<ProductInShoppingCart>()
            {
                new ProductInShoppingCart
                {
                    ProductId = id,
                    Product = new Product()
                    {
                        ProductName = "Kafe",
                    ProductImage = "https://image.shutterstock.com/image-photo/cappuccino-milk-coffee-bean-on-260nw-791935171.jpg",
                    ProductDescription = "Malo makiato so izobilni vkusovi na zrno tursko kafe",
                    ProductPrice = 120,
                    Rating = 10
                    },
                    ShoppingCartId = shoppingcartid,
                    Quantity = 6
                }
            };

            ShoppingCartDto dto = new ShoppingCartDto()
            {
                Products = listProducts,
                TotalPrice = 100
            };

            _shoppingCartService.Setup(z => z.getShoppingCartInfo(userid.ToString())).Returns(dto);

            var result = _shoppingCartController.Index(id.ToString(), " ");

            var isViewReuslt = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ShoppingCartDto>(isViewReuslt.ViewData.Model);
            Assert.Equal(1, model.Products.Count);
            //expected , actual
        }
    }
}

/*
 *  _productService.Setup(m => m.GetShoppingCartInfo(id)).Returns(dto);

            //ACT
            var result = productsController.AddProductToCard(id);

            //Assert
            var isViewReuslt = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AddToShoppingCardDto>(isViewReuslt.ViewData.Model);
            Assert.Equal(model.ProductId, id);
 * 
 * 
 * 
 */