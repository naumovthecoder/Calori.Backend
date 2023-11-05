using System;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Calori.Application;
using Calori.Application.Common.Mappings;
using Calori.Application.Interfaces;
using Calori.Domain.Models.Auth;
using Calori.WebApi.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stripe;

namespace Calori.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(config =>
            {
                config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
                config.AddProfile(new AssemblyMappingProfile(typeof(ICaloriDbContext).Assembly));
            });
            
            services.AddMvc().AddNewtonsoftJson();
            
            StripeConfiguration.AppInfo = new AppInfo
            {
                Name = "stripe-samples/checkout-single-subscription",
                Url = "https://github.com/stripe-samples/checkout-single-subscription",
                Version = "0.0.1",
            };

            services.Configure<StripeOptions>(options =>
            {
                options.PublishableKey = "pk_test_51O4pkFHNRk8vDVhu0nbecVWornSui8n7O3nmHjIPTzxdWQaJXXmaIuTlrB6H4Wz1uIBTNpG45OK3SLyL0ebclxhD00tRPVibKj";
                options.SecretKey = "sk_test_51O4pkFHNRk8vDVhuW9t5LMlX1niwMspxiCbECH1JtH8LOqI8T4ZmwMMEwNtNye2goX5orybHrP1xDm2BQAc6CbMK00vAAVMLpn";
                options.WebhookSecret = "whsec_0fba8cc180177c2f6eb0ddfccfe32430a0b5165f3da65362f4b7615e33b37a7";
                options.BasicPrice = "price_1O7hQCHNRk8vDVhuTqLeBqO6";
                options.ProPrice = "price_1O7hQCHNRk8vDVhu0d4qmdoG";
                options.Domain = "https://localhost:5001";
            });

            services.AddApplication();
            //services.AddPersistence(Configuration);
            // services.AddEmailService(Configuration);
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            });
            
            services.AddSingleton<IEmailService, EmailService.EmailService>();
            services.AddScoped<ICaloriDbContext>(provider => provider.GetService<CaloriDbContext>());


            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
                    "Data Source=testcaloriapi.ru;Initial Catalog=CaloriIdentity;User ID=sa;Password=nExa92cF;TrustServerCertificate=True;"));
            services.AddDbContext<CaloriDbContext>(options => options.UseSqlServer(
                    "Data Source=testcaloriapi.ru;Initial Catalog=Calori;User ID=sa;Password=nExa92cF;TrustServerCertificate=True;"));

            services.AddControllers();
            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Calori.WebApi",
                    Description = "Authentication and Authorization in Calori.WebApi with JWT and Swagger"
                });
                // To Enable authorization using Swagger (JWT)
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        "Enter ‘Bearer’ [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            // For Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Adding Authentication
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })

                // Adding Jwt Bearer
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = Configuration["JWT:ValidAudience"],
                        ValidIssuer = Configuration["JWT:ValidIssuer"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:SecretKey"]))
                    };
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Calori.WebApi v1"));
            }
            
            // ONLY DEV
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Calori.WebApi v1"));
            // ONLY DEV
            
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            
            // TODO: TEST
            app.UseFileServer();
            // TODO: TEST
            
            
            app.UseRouting();
            // поменял местами эти 
            app.UseHttpsRedirection();

            app.UseCors("AllowAll");
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}