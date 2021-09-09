using EShop.Domain.DomainModels;
using EShop.Domain.Idenitity;
using EShop.Repository.Interface;
using EShop.Services.Interface;
using EShop.Web.Controllers.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private Mock<IUserRepository> _userRepository;
        private AdminController adminController;

        public APIAdminTest()
        {
            this._orderService = new Mock<IOrderService>();
            this._userRepository = new Mock<IUserRepository>();
            this.adminController = new AdminController(_orderService.Object, _userRepository.Object);
        }
        
        [Fact]
        public void GetOrders_Test()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            List<Order> orders = new List<Order>()
            {
                new Order()
                {
                    Id = id1
                },
                new Order()
                {
                    Id = id2
                }
            };

            _orderService.Setup(z => z.getAllOrders()).Returns(orders);

            var result = adminController.GetOrders();

            Assert.Equal(orders, result);
        }

        [Fact]
        public void DetailsTestAPI()
        {
            var id = Guid.NewGuid();
            var id1 = Guid.NewGuid();

            Order order = new Order()
            {
                Id = id,
                ProductInOrders = null,
                User = null,
                UserId = id1.ToString()
            };

            _orderService.Setup(z => z.getOrderDetails(order));

            var result = adminController.GetDetailsForProduct(order);
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }
    }
}