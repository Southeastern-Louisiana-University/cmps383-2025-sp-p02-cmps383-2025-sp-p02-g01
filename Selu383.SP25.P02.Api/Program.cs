using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Selu383.SP25.P02.Api.Data;
using Selu383.SP25.P02.Api.Features.Users;
using Selu383.SP25.P02.Api.Features.Theaters.Roles;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.Authorization;

namespace Selu383.SP25.P02.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DataContext") ?? throw new InvalidOperationException("Connection string 'DataContext' not found.")));

            //  Adds Identity configuration 
            builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddControllers();
            builder.Services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAssertion(_ => true)
                    .Build();
            });
            // Add Swagger configuration
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Theater API", Version = "v1" });
                });
            }

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password 
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                // Lockout 
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                // User 
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Cookie 
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;

                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });
            builder.Services.AddOpenApi();
            var app = builder.Build();

            // Modified database initialization and seeding
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                await db.Database.MigrateAsync();

                // Seeds users and roles first
                await SeedUsers.Initialize(scope.ServiceProvider);

                //Seeds theaters
                SeedTheaters.Initialize(scope.ServiceProvider);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Theater API v1"));
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            if (app.Environment.IsDevelopment())
            {
                // Only proxy non-API requests to the SPA dev server
                app.MapWhen(
                    context => !context.Request.Path.StartsWithSegments("/api"),
                    builder => builder.UseSpa(spa =>
                    {
                        spa.UseProxyToSpaDevelopmentServer("http://localhost:5173");
                    })
                );
            }
            else
            {
                app.MapFallbackToFile("/index.html");
            }

            app.Run();
        }
    }
}
