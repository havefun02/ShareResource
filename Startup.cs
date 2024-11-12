using System.Text;
using System;
using ShareResource.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication;
using CRUDFramework.Cores;
using CRUDFramework.Interfaces;
using ShareResource.Interfaces;
using ShareResource.Services;
using ShareResource.Models.Entities;
using ShareResource.Middlewares;
using ShareResource.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using JwtCookiesScheme.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using static ShareResource.Policies.RBACPolicies;
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
            //services.Configure<FormOptions>(options =>
            //{
            //    options.MultipartBodyLengthLimit = 1; // Set limit to 100 MB
            //});
            services.AddWebOptimizer(pipeline =>
            {
                pipeline.AddCssBundle("css/bundle.css", "css/main.css");
                pipeline.AddJavaScriptBundle("js/bundle.js", "js/site.js");
                pipeline.MinifyCssFiles();
                pipeline.MinifyJsFiles();
            });
            services.AddSingleton<IMapper>(provider =>
            {
                var configuration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<Mapping>();
                });
                return configuration.CreateMapper();
            });
            services.AddMemoryCache();
            services.AddSingleton<RolePermissionsCacheService>();
            services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler<AdminOnlyRequirement>>();
            services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler<OwnerOnlyRequirement>>();
            services.AddDataProtection()
               .SetApplicationName("AuthenticationApp")
              .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Temp\Keys"))
              .SetDefaultKeyLifetime(TimeSpan.FromDays(30));
            services.AddScoped<IUserService<User>, UserService>();
            services.AddScoped<ITokenService<Token>, TokenService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IAdminService<User>, UserService>();
            services.AddScoped<IRoleService<Role>, RoleService>();
            services.AddScoped<IAuthService<User>, AuthService>();
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
            services.AddAuthentication("JWT-COOKIES-SCHEME").AddScheme<AuthenticationSchemeOptions, AuthenticationScheme>("JWT-COOKIES-SCHEME", null);
            services.AddAuthorization(option =>
            {
                option.AddPolicy("AdminOnly", policy => policy.Requirements.Add(new AdminOnlyRequirement(services.BuildServiceProvider().GetService<RolePermissionsCacheService>())));
                option.AddPolicy("ExecuteOnly", policy => policy.Requirements.Add(new ExecutePermissionOnly(services.BuildServiceProvider().GetService<RolePermissionsCacheService>())));

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
            app.UseWebOptimizer();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseMiddleware<LoggerMiddleware>();
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