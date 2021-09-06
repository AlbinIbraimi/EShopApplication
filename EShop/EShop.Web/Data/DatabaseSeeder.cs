using EShop.Domain.DomainModels;
using EShop.Domain.DTO;
using EShop.Domain.Idenitity;
using EShop.Repository;
using EShop.Services.Interface;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.IntegrationTest.Data
{
    public class DatabaseSeeder
    {
        private readonly UserManager<EShopApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IProductService _productService;
        public DatabaseSeeder(ApplicationDbContext context, UserManager<EShopApplicationUser> userManager, IProductService productService) 
        {
            this._userManager = userManager;
            this._context = context;
            this._productService = productService;
        }

        public async Task Seed()
        { 
            //Add all user profiles
            foreach (var user in PredefinedData.users)
            {
                user.UserCart = new ShoppingCart()
                {
                    Id = Guid.NewGuid(),
                    Owner = user,
                    OwnerId = user.Id
                };
                await _userManager.CreateAsync(user, PredefinedData.password);
            }

            //Add all products to the db
            _context.Products.AddRange(PredefinedData.products);
            _context.SaveChanges();

            //Add products to the shopingcart of the first user;
            var id = PredefinedData.users[0].Id;
            for(int i=0; i<3; i++)
            {
                var product = PredefinedData.products[i];
                AddToShoppingCardDto dto = new AddToShoppingCardDto()
                {
                    SelectedProduct = product,
                    ProductId = product.Id,
                    Quantity = 1
                };
                _productService.AddToShoppingCart(dto, id);
            }

        }

    }
}
