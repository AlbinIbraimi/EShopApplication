using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using Xunit;

namespace EshopApplicationEndToEndTest
{
    public class EndToEndTesting
    {

        private IWebDriver driver;

        public EndToEndTesting()
        {
            ChromeOptions options = new ChromeOptions();
            options.AcceptInsecureCertificates = true;
            this.driver = new ChromeDriver(@"C:\Users\Albin\Desktop\Finki\SKIT\Proekt\EshopApplicationEndToEndTest\EshopApplicationEndToEndTest\ChromDriver\", options);
        }

        [Fact]
        public void HomePage()
        {
            // Set the driver to open EShopApplication
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            driver.Navigate().GoToUrl("https://localhost:44309/");
            Assert.Equal("Home Page - EShop.Web", driver.Title);
            Thread.Sleep(1000);
            driver.Close();

        }

        [Fact]
        public void LoginTest()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            driver.Navigate().GoToUrl("https://localhost:44309/");
            bool login = loginFucntion("albin@test.com", "Albin123!");

            Assert.Equal("Home Page - EShop.Web", driver.Title);
            Assert.True(login);
            Thread.Sleep(1000);
            driver.Close();

        }
        private bool loginFucntion(string email, string password) 
        {
            var loginButton = driver.FindElement(By.CssSelector("a[href=\"/account/login\"]"));
            loginButton.Click();

            var EmilInputBox = driver.FindElement(By.CssSelector("input[name=\"Email\"]"));
            EmilInputBox.SendKeys(email);
            var PasswordInputBox = driver.FindElement(By.CssSelector("input[name=\"Password\"]"));
            PasswordInputBox.SendKeys(password);
            driver.FindElement(By.CssSelector("button[type=\"submit\"]")).Click();

            var userEmail = driver.FindElement(By.CssSelector("a[title=\"Manage\"]"));
            return userEmail.Text.Equals($"Hello {email}!");

        }

        [Fact]
        public void RegisterTest() 
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            driver.Navigate().GoToUrl("https://localhost:44309/");
            driver.FindElement(By.CssSelector("a[href=\"/account/Register\"]")).Click();

            driver.FindElement(By.CssSelector("input[name=\"Email\"]")).SendKeys("test@test.com");
            driver.FindElement(By.CssSelector("input[name=\"Password\"]")).SendKeys("Test123!");
            driver.FindElement(By.CssSelector("input[name=\"ConfirmPassword\"]")).SendKeys("Test123!");

            driver.FindElement(By.CssSelector("button[type=\"submit\"]")).Click();

            bool login = loginFucntion("test@test.com", "Test123!");
            Assert.True(login);
            Thread.Sleep(1000);
            driver.Close();
        }
        private void DeleteIfExist(string cardTitle) 
        {
            var cardElements = driver.FindElements(By.CssSelector("h3[class=\"card-title\"]"));
            foreach(var card in cardElements)
            {
                if (card.Text.Trim().Equals(cardTitle))
                {
                    driver.FindElement(By.CssSelector($"a[cardtitle=\"{cardTitle}\"]")).Click();
                    driver.FindElement(By.CssSelector("input[value=\"Delete\"]")).Click();
                }
            }
        }
        [Fact]
        public void AddNewProduct_DeleteOldProductTest() 
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            driver.Navigate().GoToUrl("https://localhost:44309/");
            bool login = loginFucntion("albin@test.com", "Albin123!");
            driver.Navigate().GoToUrl("https://localhost:44309/Products");

            DeleteIfExist("New Product");
            driver.FindElement(By.CssSelector("a[href=\"/Products/Create\"]")).Click();
            driver.FindElement(By.CssSelector("input[name=\"ProductName\"]")).SendKeys("New Product");
            driver.FindElement(By.CssSelector("input[name=\"ProductImage\"]")).SendKeys("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTUK1LaBaummJfuW6GIM_kt3R9egIlpqVpEKw&usqp=CAU");
            driver.FindElement(By.CssSelector("input[name=\"ProductDescription\"]")).SendKeys("Sonce");
            driver.FindElement(By.CssSelector("input[name=\"ProductPrice\"]")).SendKeys("100");
            driver.FindElement(By.CssSelector("input[name=\"Rating\"]")).SendKeys("10");
            driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();
            Thread.Sleep(1000);
            Assert.Equal("Index - EShop.Web", driver.Title);
            var cardElements =  driver.FindElements(By.CssSelector("h3[class=\"card-title\"]"));

            IWebElement element = null;
            foreach(var card in cardElements)
            {
                if(card.Text.Equals("New Product"))
                {
                    element = card;
                };
            }

            Assert.Equal("New Product", element.Text.Trim());
            Thread.Sleep(1000);
            driver.Close();
        }

        public static IWebElement GetParent(IWebElement node) 
        {
            return node.FindElement(By.XPath(".."));
        }

        private void DeleteFromShopingCartIfExist(string productName) 
        {
            driver.FindElement(By.CssSelector("a[href=\"/ShoppingCart\"]")).Click();
            var elements = driver.FindElements(By.CssSelector("td"));

            IWebElement product = null;
            foreach(var element in elements)
            {
                if (element.Text.Trim().Equals(productName))
                    product = element; 
            }
            if(product!=null)
            {
                IWebElement parent = GetParent(product);
                parent.FindElement(By.CssSelector("a[class=\"btn btn-danger\"]")).Click();
            }
        }
        [Fact]
        public void AddProductToShopingCart_Test() 
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            driver.Navigate().GoToUrl("https://localhost:44309/");
            bool login = loginFucntion("albin@test.com", "Albin123!");
            driver.Navigate().GoToUrl("https://localhost:44309/Products");
            DeleteFromShopingCartIfExist("New Product");

            var node = driver.FindElement(By.CssSelector("a[cardtitle=\"New Product\"]"));
            var paretn = GetParent(node);
            paretn.FindElement(By.CssSelector("a[class=\"btn btn-info\"]")).Click();
            driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();

            driver.Navigate().GoToUrl("https://localhost:44309/ShoppingCart");
            var elements = driver.FindElements(By.CssSelector("td"));

            IWebElement product = null;
            foreach (var element in elements) 
            {
                if (element.Text.Trim().Equals("New Product"))
                    product = element;
            }

            Assert.Equal("New Product", product.Text.Trim());
            Thread.Sleep(1000);
            driver.Close();

        }


    }
}
