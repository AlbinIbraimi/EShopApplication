using EShop.Domain.DomainModels;
using EShop.Domain.Idenitity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.IntegrationTest.Data
{
    public class PredefinedData
    {

        public static string password = "EShopApp123!";

        public static EShopApplicationUser[] users = new[]
        {
            new EShopApplicationUser()
            {
               Email = "albin@test.com",
               UserName = "albin@test.com",
               FirstName = "Albin",
               LastName = "Ibraimi"
            },
            new EShopApplicationUser()
            {
                Email  = "david@test.com",
                UserName = "david@test.com",
                FirstName = "David",
                LastName = "Anastasov"
            }
        };

        public static Product[] products = new[]
        {
            new Product()
            {
                ProductName = "Kokos",
                ProductDescription = "Kokosot e najdobro ovoshje",
                ProductImage = "https://stil.kurir.rs/data/images/2020/03/30/19/215607_kokos-shutter_ff.jpg",
                ProductPrice = 100,
                Rating = 4
            },
            new Product()
            {
                    ProductName = "Kafe",
                    ProductImage = "https://image.shutterstock.com/image-photo/cappuccino-milk-coffee-bean-on-260nw-791935171.jpg",
                    ProductDescription = "Malo makiato so izobilni vkusovi na zrno tursko kafe",
                    ProductPrice = 120,
                    Rating = 10
            },
             new Product()
            {
                  ProductName = "Cokolada",
                    ProductImage = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f2/Chocolate.jpg/250px-Chocolate.jpg",
                    ProductDescription = "Cokolatce za so kafe",
                    ProductPrice = 120,
                    Rating = 10
            },
              new Product()
            {

                    ProductName = "Krastavica",
                    ProductImage = "https://agrohemija.com.mk/wp-content/uploads/2018/05/Krastavica-dolga.jpg",
                    ProductDescription = "Krastavicka....!!",
                    ProductPrice = 120,
                    Rating = 10
            },
               new Product()
            {

                    ProductName = "Kukla",
                    ProductImage = "https://n4.sdlcdn.com/imgs/a/v/5/Barbie-Barbie-Basic-Dol-SDL185936149-1-c28a1.jpg",
                    ProductDescription = "Kukla so magija!",
                    ProductPrice = 120,
                    Rating = 10
            }
        };


    }
}
