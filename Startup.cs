﻿using System.Text;
using System;
using ShareResource.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication;
using CRUDFramework.Cores;
using CRUDFramework.Interfaces;
using ShareResource.Policies;
using ShareResource.Interfaces;
using ShareResource.Services;
using ShareResource.Models.Entities;
using ShareResource.Middlewares;
using ShareResource.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace ShareResource
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<AppDbContext>();
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddSingleton<IMapper>(provider =>
            {
                var configuration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<Mapping>();
                });
                return configuration.CreateMapper();
            });
            services.AddScoped<IUserService<User>, UserService>();
            services.AddScoped<IAdminService<User>, UserService>();
            services.AddScoped<IRoleService<Role>, RoleService>();
            services.AddScoped<IAuthService<User, Token>, AuthService>();
            services.AddScoped<IResourceMod<Img>, ResourceModService>();
            services.AddScoped<IResourceAccess<Img>, ResourceAccessService>();
            services.AddScoped<IPaginationService<Img>, OffsetPaginationService<Img>>();
            services.AddHttpContextAccessor();

            services.AddSingleton<IJwtService<User>,JwtService>();


            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            services.AddAuthentication("JWT-COOKIES-SCHEME").AddScheme<AuthenticationSchemeOptions, AppAuthenticationHandler>("JWT-COOKIES-SCHEME", null);
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy("OwnerOnly", policy =>
            policy.RequireRole("Owner"));


            });
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "DEVELOPMENT API", Version = "v1" }); });
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseCors("AllowAll");
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseMiddleware<LoggerMiddleware>();
            app.UseMiddleware<FileValidationMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStatusCodePages(async context =>
            {
                var response = context.HttpContext.Response;
                var statusCode = response.StatusCode;

                if (statusCode == 404)
                {
                    response.ContentType = "text/html";
                    await response.SendFileAsync("wwwroot/notFoundPage.html");
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                });
        }
    }
}