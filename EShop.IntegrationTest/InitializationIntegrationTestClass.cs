using EShop.Domain.Idenitity;
using EShop.IntegrationTest.Data;
using EShop.Repository;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace EShop.IntegrationTest
{
   public class InitializationIntegrationTestClass : IDisposable
    {
        protected  TestServer _testServer;
        protected  HttpClient _testClient;

        protected ApplicationDbContext _context;
        protected DatabaseSeeder _seeder;
        protected SetCookieHeaderValue _antiforgeryCookie;
        protected SetCookieHeaderValue _authenticationCookie;
        protected string _antiforgeryToken;

        protected static Regex AntiforgeryFormFieldRegex = new Regex(@"\<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" \/\>");

        public InitializationIntegrationTestClass()
        {
            CreateTestServerAndClient();
            CreateContext();
            CreateSeeder();
            _seeder.Seed().Wait();
        }

        public void CreateContext() 
        {
            var serviceScope = _testServer.Services.GetRequiredService<IServiceProvider>().CreateScope();
            _context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }
        public void CreateSeeder() 
        {
            var serviceScope = _testServer.Services.GetRequiredService<IServiceProvider>().CreateScope();
            _seeder = serviceScope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        }
        public void CreateTestServerAndClient() 
        {
            _testServer = new TestServer(WebHost.CreateDefaultBuilder()
                   .UseStartup<EShop.Web.TestStartup>()
                   .UseEnvironment("Development"));
            _testClient = _testServer.CreateClient();

        }

        public void Dispose()
        {
            _testClient.Dispose();
            _testServer.Dispose();
            _context.Dispose();
        }

        public void RecreateTestServerAndClient() 
        {
            Dispose();
            CreateTestServerAndClient();
            CreateContext();
            CreateSeeder();
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _seeder.Seed().Wait();
        }
        protected string EnsureAntiforgeryToken()
        {
            if (_antiforgeryToken != null) return _antiforgeryToken;

            var response =  _testClient.GetAsync("/Account/Login").Result;
            response.EnsureSuccessStatusCode();
            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
            {
                _antiforgeryCookie = SetCookieHeaderValue.ParseList(values.ToList()).SingleOrDefault(c => c.Name.StartsWith(".AspNetCore.AntiForgery.", StringComparison.InvariantCultureIgnoreCase));
            }
            Assert.NotNull(_antiforgeryCookie);
            _testClient.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(_antiforgeryCookie.Name, _antiforgeryCookie.Value).ToString());

            var responseHtml =  response.Content.ReadAsStringAsync().Result;
            var match = AntiforgeryFormFieldRegex.Match(responseHtml);
            _antiforgeryToken = match.Success ? match.Groups[1].Captures[0].Value : null;
            Assert.NotNull(_antiforgeryToken);

            return _antiforgeryToken;
        }

        protected Dictionary<string, string> EnsureAntiforgeryTokenForm(Dictionary<string, string> formData = null)
        {
            if (formData == null) formData = new Dictionary<string, string>();

            formData.Add("__RequestVerificationToken",  EnsureAntiforgeryToken());
            return formData;
        }

        public void EnsureAuthenticationCookie()
        {
            if (_authenticationCookie != null) return;

            var formData =  EnsureAntiforgeryTokenForm(new Dictionary<string, string>
    {
        { "Email", PredefinedData.users[0].Email },
        { "Password", PredefinedData.password }
    });
            var response = _testClient.PostAsync("/Account/Login", new FormUrlEncodedContent(formData)).Result;
            //Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
            {
                foreach(var value in values)
                {
                    Console.WriteLine("----" + value.ToString());
                }
                _authenticationCookie = SetCookieHeaderValue.ParseList(values.ToList()).SingleOrDefault(c => c.Name.StartsWith("AUTHENTICATION_COOKIE", StringComparison.InvariantCultureIgnoreCase));
            }
            Assert.NotNull(_authenticationCookie);
            _testClient.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(_authenticationCookie.Name, _authenticationCookie.Value).ToString());

            // The current pair of antiforgery cookie-token is not valid anymore
            // Since the tokens are generated based on the authenticated user!
            // We need a new token after authentication (The cookie can stay the same)
            _antiforgeryToken = null;
        }
    }
}
