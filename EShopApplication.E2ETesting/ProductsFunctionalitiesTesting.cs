using System;
using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace EShopApplication.E2ETesting
{
    public class ProductsFunctionalitiesTesting
    {
        private IWebDriver driver;
        public ProductsFunctionalitiesTesting() 
        {
            this.driver = new FirefoxDriver();
        }
        [Fact]
        public void Test1()
        {
            //Set the driver to open the application
            driver.Navigate().GoToUrl("https://localhost:44309/");
            driver.FindElement(By.CssSelector("input")).SendKeys("Selenium");
            driver.FindElement(By.CssSelector("input")).SendKeys(Keys.Enter);
        }
    }
}
