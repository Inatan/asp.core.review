﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Alura.WebAPI.WebApp.Formatters;
using System;
using Alura.ListaLeitura.HttpClients;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Alura.ListaLeitura.WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Usuario/Login";
                });

            services.AddHttpClient<LivroApiClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:6000/api/");
            });

            services.AddHttpClient<AuthApiClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");
            });

            services.AddHttpContextAccessor();

            services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/Usuario/Login";
            });

            services.AddMvc(options =>{
                options.OutputFormatters.Add(new LivroCsvFormatter());
            }).AddXmlSerializerFormatters();

            


        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //using (var serviceScope = app.ApplicationServices
            //.GetRequiredService<IServiceScopeFactory>()
            //.CreateScope())
            //{
            //    using (var context = serviceScope.ServiceProvider.GetService<LeituraContext>())
            //    {
            //        context.Database.Migrate();
            //    }
            //}

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
