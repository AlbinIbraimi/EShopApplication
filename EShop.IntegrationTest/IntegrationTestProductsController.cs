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
            lock (this)
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
        }

        [Fact]
        public void Index_Get_ReturnsIndexHtmlPageAsync()
        {
            lock (this)
            {
                //ACT

                var response = _testClient.GetAsync("/Products/Index").Result;

                //ASSERT

                response.EnsureSuccessStatusCode();
                var responseString = response.Content.ReadAsStringAsync().Result;
                foreach (var Product in PredefinedData.products)
                {
                    Assert.Contains($"<div class=\"card\" data-productId=\"{Product.Id}\" style=\"width: 18rem; height: 30rem;\">", responseString);
                }
            }
        }

        [Fact]
        public void Create_PostMethode_RedirectToIndex()
        {
            lock (this)
            {
                this.RecreateTestServerAndClient();


                //Arange
                var result = EnsureAntiforgeryToken();
                var id = Guid.NewGuid();

                var formData = EnsureAntiforgeryTokenForm(new Dictionary<string, string>
            {
                { "Id", id.ToString()},
                { "ProductName" , "bana" },
                { "ProductDescription" , "bana bana"},
                { "ProductImage", "BLABLABLA"},
                { "ProductPrice", "100"},
                { "Rating", "100"}
            });

                //ACT
                var response = _testClient.PostAsync("/Products/Create", new FormUrlEncodedContent(formData)).Result;

                //Assert
                Assert.Equal(HttpStatusCode.Found, response.StatusCode);
                Assert.Equal("/Products", response.Headers.Location.ToString());
            }
        }

        [Fact]
        public async Task AddProductToShopingCart_PostMethode_RequireAuthorization()
        {

            this.RecreateTestServerAndClient();
            EnsureAntiforgeryToken();


            //Arange
            //Add new product to the database with known Id
            Guid id = Guid.NewGuid();
            var productData = EnsureAntiforgeryTokenForm(new Dictionary<string, string>
            {
                 { "Id", id.ToString()},
                { "ProductName", "CD"},
                { "ProductImage", "https://image.shutterstock.com/image-vector/cd-disk-vector-illustration-260nw-577991263.jpg"},
                { "ProductDescription", "Empty"},
                { "ProductPrice", "0"},
                { "Rating", "5"}
            });

            var response = await _testClient.PostAsync("Products/Create", new FormUrlEncodedContent(productData));
            Assert.Equal(HttpStatusCode.Found, response.StatusCode);

            //ACT
            //Add the product to the shoping cart
            var AddToShopingCartDto = new Dictionary<string, string>()
            {
                { "ProductId", id.ToString()},
                { "Quantity", "2"}
            };

            var result = await _testClient.PostAsync("/Products/AddProductToCard", new FormUrlEncodedContent(AddToShopingCartDto));
            Assert.Equal(HttpStatusCode.Found, result.StatusCode);

        }

        [Fact]
        public void Test_DetailsAction_ReturnsInformationForProduct() 
        {
            this.RecreateTestServerAndClient();

            var product = PredefinedData.products[0];

            //ACT
            var result = _testClient.GetAsync($"/Products/Details/{product.Id}").Result;

            //Assert

            var responseString = result.Content.ReadAsStringAsync().Result;
            Assert.Contains($"<input type=\"hidden\" name=\"NameOfTheProduct\" value=\"{ product.ProductName}\" />", responseString);
        }

        [Fact]
        public void Test_EditPostMethode_ReturnsRedirectToActionIndex()
        {
            this.RecreateTestServerAndClient();
            var tmp = this.EnsureAntiforgeryToken();

            var product = PredefinedData.products[0];
            
            //Update the product end send the new product
            product.ProductName = "kokosovo maslo";
            var data = EnsureAntiforgeryTokenForm(new Dictionary<string, string>()
            {
                { "ProductName" , product.ProductName },
                { "ProductDescription" , product.ProductDescription},
                { "ProductImage", product.ProductImage},
                { "ProductPrice", product.ProductPrice.ToString()},
                { "Rating", product.Rating.ToString() }
            });

            var responseEdit = _testClient.PostAsync($"/Products/Edit/{product.Id}", new FormUrlEncodedContent(data)).Result;
            Assert.Equal(HttpStatusCode.Found, responseEdit.StatusCode);

            //Check the name of product after update;
            var result = _testClient.GetAsync($"/Products/Details/{product.Id}").Result;
            var responseString = result.Content.ReadAsStringAsync().Result;
            Assert.Contains($"<input type=\"hidden\" name=\"NameOfTheProduct\" value=\"{ product.ProductName}\" />", responseString);

        }

        [Fact]
        public void Test_DeleteProduct_ReturnRedirectToActionIndex() 
        {
            this.RecreateTestServerAndClient();
            var tmp = this.EnsureAntiforgeryToken();

            var product = PredefinedData.products[0];
            var formData = this.EnsureAntiforgeryTokenForm();

            //ACT
            var result = _testClient.PostAsync($"/Products/Delete/{product.Id}", new FormUrlEncodedContent(formData));
            Assert.Equal(HttpStatusCode.Found, result.Result.StatusCode);

            //Check if the data is deleted;
            var checkForEdit = _testClient.GetAsync($"/Products/Edit/{product.Id}");
            Assert.Equal(HttpStatusCode.NotFound, checkForEdit.Result.StatusCode);
           
        }
}
}
