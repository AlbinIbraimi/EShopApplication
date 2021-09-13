using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using Xunit;

namespace EshopApplicationEndToEndTest
{
    public class TestProductFunctionalities
    {

        private IWebDriver driver;

        public TestProductFunctionalities()
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

        }

        [Fact]
        public void LoginTest()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            driver.Navigate().GoToUrl("https://localhost:44309/");
            var loginButton = driver.FindElement(By.CssSelector("a[href=\"/account/login\"]"));
            loginButton.Click();
            var EmilInputBox = driver.FindElement(By.CssSelector("input[name=\"Email\"]"));
            EmilInputBox.SendKeys("albin@test.com");
            var PasswordInputBox = driver.FindElement(By.CssSelector("input[name=\"Password\"]"));
            PasswordInputBox.SendKeys("Albin123!");

            driver.FindElement(By.CssSelector("button[type=\"submit\"]")).Click();
            Assert.Equal("Home Page - EShop.Web", driver.Title);
            var userEmail = driver.FindElement(By.CssSelector("a[title=\"Manage\"]"));
            Assert.Equal("Hello albin@test.com!", userEmail.Text);


        }
    }
}
