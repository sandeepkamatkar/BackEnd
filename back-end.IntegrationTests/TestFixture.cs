using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using back_end.Data;

namespace back_end.IntegrationTests
{
    public class TestFixture : IDisposable
    {
        private readonly TestServer _server;

        public HttpClient Client { get; }

        public TestFixture()
        {
            var builder = new WebHostBuilder()
                .UseStartup<back_end.Startup>()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "..\\..\\..\\..\\back-end"
                    ));

                    config.AddJsonFile("appsettings.json");
                })
                .ConfigureServices(services => {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                            typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null) {
                        services.Remove(descriptor);
                    }
                    services.AddDbContext<ApplicationDbContext>(options => {
                        options.UseInMemoryDatabase("IntegrationDB");
                    });

                    var sp = services.BuildServiceProvider();

                    // using (var scope = sp.CreateScope())
                    // {
                    //     var scopedServices = scope.ServiceProvider;
                    //     var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                       
                    //     // Ensure the database is created.
                    //     db.Database.EnsureCreated();

                    //     try
                    //     {
                    //         // Seed the database with test data.
                    //         Utilities.InitializeDbForTests(db);
                    //     }
                    //     catch (Exception ex)
                    //     {
                    //         Console.WriteLine(ex.StackTrace + "\nAn error occurred seeding the " +
                    //             "database with test messages.\nError: {Message}", ex.Message);
                    //     }
                    // }


                });
                

            _server = new TestServer(builder);

            Client = _server.CreateClient();
            Client.BaseAddress = new Uri("https://localhost:8888");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }
}

