using EShop.Domain.DomainModels;
using EShop.Domain.Idenitity;
using EShop.Repository;
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

        public DatabaseSeeder(ApplicationDbContext context, UserManager<EShopApplicationUser> userManager) 
        {
            this._userManager = userManager;
            this._context = context;
        }

        public async Task Seed()
        { 
            //Add all user profiles
            foreach (var user in PredefinedData.users)
            {
                user.UserCart = new ShoppingCart();
                await _userManager.CreateAsync(user, PredefinedData.password);
            }

            //Add all products to the db
            _context.Products.AddRange(PredefinedData.products);
            _context.SaveChanges();
        }

    }
}
