using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ApiClick.Configurations;
using ApiClick.Controllers.FrequentlyUsed;
using ApiClick.StaticValues;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ShopAdminAPI.Configurations;
using ShopAdminAPI.Controllers;
using ShopHubAPI.StaticValues;

namespace ApiClick
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            ApiConfiguration.SHOP_ID = Configuration["ShopConfig:ShopId"];
            ApiConfiguration.SHOP_HUB_API = Configuration["ShopConfig:ShopHubAPI"];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ShopContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // ????????, ????? ?? ?????????????? ???????? ??? ????????? ??????
                            ValidateIssuer = true,
                            // ??????, ?????????????? ????????
                            ValidIssuer = ApiConfiguration.SHOP_ID,

                            ValidateAudience = false,
                            // ????????? ??????????? ??????

                            ValidateLifetime = false,
                            // ????????? ????? ????????????
                            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                            // ????????? ????? ????????????
                            ValidateIssuerSigningKey = true,
                        };
                    });

            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                }
            );

            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                 endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            }); 
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //Getting configuration
            IHttpClientFactory _clientFactory = app.ApplicationServices.GetService(typeof(IHttpClientFactory)) as IHttpClientFactory;

            var trash = new ConfigController(null, null);

            lifetime.ApplicationStarted.Register(trash.GetShopConfig(_clientFactory).Wait);
        }
    }
}
