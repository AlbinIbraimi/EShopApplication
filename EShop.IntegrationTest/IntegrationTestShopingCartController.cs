using EShop.IntegrationTest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EShop.IntegrationTest
{
    public class IntegrationTestShopingCartController : InitializationIntegrationTestClass
    {
        [Fact]
        public void Test_IndexMethode()
        {
            var userId = PredefinedData.users[0].Id;
            //Act
            var resultOfIndexAction = _testClient.GetAsync($"/ShoppingCart/Index?id={userId.ToString()}&test={true.ToString()}").Result;
            Assert.Equal(HttpStatusCode.OK, resultOfIndexAction.StatusCode);
            var responseString = resultOfIndexAction.Content.ReadAsStringAsync().Result;
            for (int i = 0; i < 3; i++)
            {
                Assert.Contains($"<td product-id=\"{PredefinedData.products[i].Id}\">{PredefinedData.products[i].ProductName}</td>", responseString);
            }

        }

        [Fact]
        public void Test_DeleteProductFromShoppingCart_returnsRedirectToAction() 
        {
            this.RecreateTestServerAndClient();
            //ARANGE
            var userId = PredefinedData.users[0].Id;
            var productToDeleteId = PredefinedData.products[0].Id;

            var result = _testClient.GetAsync($"/ShoppingCart/DeleteFromShoppingCart?id={productToDeleteId.ToString()}&mockUserId={userId.ToString()}&test=True").Result;
            var resultToSTring = result.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.Found, result.StatusCode);
            Assert.DoesNotContain($"<td product-id=\"{PredefinedData.products[0].Id}\">{PredefinedData.products[0].ProductName}</td>", resultToSTring);
        }

    }
}
