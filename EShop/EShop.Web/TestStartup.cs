using EShop.IntegrationTest.Data;
using EShop.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Web
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration) { }

        public override void ConfigureDataBase(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("EShop_test_db"));

            //Register the database seeder
            services.AddTransient<DatabaseSeeder>();
        }

        //[Obsolete]
        //public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        //{
        //    base.Configure(app, env);

        //    //Seed the database
        //    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope()) 
        //    {
        //        var seeder = serviceScope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        //        seeder.Seed().Wait();
        //    }
        //}
    }
}
