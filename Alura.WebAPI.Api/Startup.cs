﻿using System;
using System.Collections.Generic;
using Alura.ListaLeitura.Api.Formatters;
using Alura.ListaLeitura.Api.Models;
using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Alura.WebAPI.Api.Filtros;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Alura.WebAPI.Api
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
            services.AddDbContext<LeituraContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("ListaLeitura"));
            });

            services.AddTransient<IRepository<Livro>, RepositorioBaseEF<Livro>>();

            services.AddMvc(options => {
                options.OutputFormatters.Add(new LivroCsvFormatter());
                options.Filters.Add(typeof(ErrorResponseFilter));
            }).AddXmlSerializerFormatters();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            }).AddJwtBearer("JwtBearer", options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("alura-webapi-authentication-valid")),
                    ClockSkew = TimeSpan.FromMinutes(5),
                    ValidIssuer = "Alura.WebApp",
                    ValidAudience = "Postman",
                };
            });



            services.AddApiVersioning(options =>
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("api-version"),
                    new QueryStringApiVersionReader("api-version")
                )
            );

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Livros Api", Description = "Documentação da API", Version = "1.0" });
                c.SwaggerDoc("v2", new Info { Title = "Livros Api",  Description = "Documentação da API", Version = "2.0" });
                c.EnableAnnotations();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Description = "Autenticação Bearer via JWT",
                    Scheme = "Bearer"
                });
                //adicionando o filtro para incluir respostas 401 nas operações
                c.OperationFilter<AuthResponsesOperationFilter>();
                //adicionando o filtro para incluir descrições nas tags
                c.DocumentFilter<TagDescriptionsDocumentFilter>();
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v2");
            });
        }
    }
}
