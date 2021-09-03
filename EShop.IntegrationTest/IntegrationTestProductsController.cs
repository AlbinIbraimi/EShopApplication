using EShop.Domain.DomainModels;
using EShop.IntegrationTest.Data;
using EShop.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;


namespace EShop.IntegrationTest
{
    public class IntegrationTestProductsController : InitializationIntegrationTestClass
    {

        [Fact]
        public void Test_Server_Client_InMemoryDatabase() 
        {
            //Check the initial state of the inMemoryDatabase
            var numProducts = _context.Products.ToListAsync().Result.Count;
            Assert.Equal(5, numProducts);



            //Add new product to the database
            Product newProduct = new Product()
            {
                ProductName = "Banana",
                ProductDescription = "prazno mesto",
                ProductImage = "Nema Slika",
                ProductPrice = 100,
                Rating = 4
            };

            _context.Products.Add(newProduct);
            _context.SaveChanges();
            numProducts = _context.Products.ToListAsync().Result.Count;
            Assert.Equal(6, numProducts);

            //Restoring the initial state of the database by recreating the _testHost with new in memory database
            this.RecreateTestServerAndClient();

            numProducts = _context.Products.ToListAsync().Result.Count;
            Assert.Equal(5, numProducts);

        }

        [Fact]
        public async Task Index_Get_ReturnsIndexHtmlPageAsync()
        {
            //ACT

            var response = await _testClient.GetAsync("/Products/Index");

            //ASSERT

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            foreach (var Product in PredefinedData.products)
            {
                Assert.Contains($"<div class=\"card\" data-productId=\"{Product.Id}\" style=\"width: 18rem; height: 30rem;\">", responseString);
            }
        }

        [Fact]
        public async Task Create_PostMethode_RedirectToIndex()
        {
            //Arange
            await EnsureAntiforgeryToken();
            var id = Guid.NewGuid();

            var formData = await EnsureAntiforgeryTokenForm(new Dictionary<string, string>
            {
                { "Id", id.ToString()},
                { "ProductName" , "bana" },
                { "ProductDescription" , "bana bana"},
                { "ProductImage", "BLABLABLA"},
                { "ProductPrice", "100"},
                { "Rating", "100"}
            });

            //ACT
            var response = await _testClient.PostAsync("/Products/Create", new FormUrlEncodedContent(formData));

            //Assert
            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
            Assert.Equal("/Products", response.Headers.Location.ToString());
        }

        //[Fact]
        //public async Task AddProductToShopingCart_PostMethode_RequireAuthorization()
        //{
        //    //Login as existing user
        //    var loginData = new Dictionary<string, string>()
        //    {
        //        { "Email", "albin@test.com"},
        //        { "Password", "EShopApp123!" },
        //    };

        //    var loginResponse = await _testClient.PostAsync("/Account/Login", new FormUrlEncodedContent(loginData));
        //    Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);


        //    //Arange
        //    //Add new product to the database with known Id
        //    Guid id = Guid.NewGuid();
        //    var productData = new Dictionary<string, string>()
        //    {
        //        { "Id", id.ToString()},
        //        { "ProductName", "CD"},
        //        { "ProductImage", "https://image.shutterstock.com/image-vector/cd-disk-vector-illustration-260nw-577991263.jpg"},
        //        { "ProductDescription", "Empty"},
        //        { "ProductPrice", "0"},
        //        { "Rating", "5"}
        //    };

        //    var response = await _testClient.PostAsync("Products/Create", new FormUrlEncodedContent(productData));
        //    Assert.Equal(HttpStatusCode.Found, response.StatusCode);

        //    //ACT
        //    //Add the product to the shoping cart
        //    var AddToShopingCartDto = new Dictionary<string, string>()
        //    {
        //        { "ProductId", id.ToString()},
        //        { "Quantity", "2"}
        //    };

        //    var result = await _testClient.PostAsync("/Products/AddProductToCard", new FormUrlEncodedContent(AddToShopingCartDto));
        //    Assert.Equal(HttpStatusCode.Found, result.StatusCode);

        //}
    }
}
