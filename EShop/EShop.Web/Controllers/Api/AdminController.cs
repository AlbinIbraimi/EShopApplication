using EShop.Domain.DomainModels;
using EShop.Domain.Idenitity;
using EShop.Repository.Interface;
using EShop.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IOrderService _orderService;
        private readonly IUserRepository _userRepository;

        public AdminController(IOrderService orderService, IUserRepository userRepository)
        {
            this._orderService = orderService;
            this._userRepository = userRepository;
        }

        [HttpGet("[action]")]
        public List<Order> GetOrders()
        {
            return this._orderService.getAllOrders();
        }

        [HttpPost("[action]")]
        public Order GetDetailsForProduct(BaseEntity model)
        {
            return this._orderService.getOrderDetails(model);
        }

        [HttpPost("[action]")]
        public bool ImportAllUsers(List<UserRegistrationDto> model)
        {
            bool status = true;

            foreach (var item in model)
            {
                var userCheck = _userRepository.findByEmail(item.Email);
                
                if(userCheck == null)
                {
                    var user = new EShopApplicationUser
                    {
                        UserName = item.Email,
                        NormalizedUserName = item.Email,
                        Email = item.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        UserCart = new ShoppingCart()
                    };
                    var result = _userRepository.createUser(user, item.Password);

                    status = status && result.Succeeded;
                }
                else
                {
                    continue;
                }
            }

            return status;
        }

    }
}
