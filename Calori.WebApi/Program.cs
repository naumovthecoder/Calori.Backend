using System;
using System.Net;
using Calori.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;

namespace Calori.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();


            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    // var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    // db.Database.Migrate();
                    // var context = serviceProvider.GetRequiredService<CaloriDbContext>();
                    // context.Database.Migrate();
                    //DbInitializer.Initialize(context);
                }
                catch (Exception exception)
                {

                }
            }
            
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            DotNetEnv.Env.Load();
            
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseWebRoot("./client");
                    //webBuilder.UseWebRoot(".");
                    //webBuilder.UseUrls("http://0.0.0.0:4242");
                    // webBuilder.UseUrls("http://*:5000");
                    // webBuilder.UseUrls("https://*:5001");
                    // webBuilder.UseKestrel((hostingContext, options) =>
                    // {
                    //     var certificatePath = "/etc/ssl/testcaloriapi.crt";
                    //     var certificatePassword = "/etc/ssl/testcaloriapi.key";
                    //
                    //     options.Listen(IPAddress.Any, 443, listenOptions =>
                    //     {
                    //         listenOptions.UseHttps(certificatePath, certificatePassword);
                    //     });
                    // });
                });
        }
    }
}