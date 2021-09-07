using EShop.Domain.DomainModels;
using EShop.Domain.Idenitity;
using EShop.Services.Interface;
using EShop.Web.Controllers.Api;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EShop.Test.Controllers.Api
{
    public class APIAdminTest
    {
        private Mock<IOrderService> _orderService;
        private Mock<UserManager<EShopApplicationUser>> userManager;
        private AdminController adminController;

        public APIAdminTest()
        {
            this._orderService = new Mock<IOrderService>();
            this.userManager = new Mock<UserManager<EShopApplicationUser>>();
            this.adminController = new AdminController(_orderService.Object, userManager.Object);
        }

        [Fact]
        public void GetOrders_Test()
        {
           

            _orderService.Setup(z => z.getAllOrders());

            var result = adminController.GetOrders();

            Assert.Equal(tmp, result);
        }
    }
}