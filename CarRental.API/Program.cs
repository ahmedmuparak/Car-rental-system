using CarRental.Application.Configuration;
using CarRental.Application.DTOs.Auth;
using CarRental.Application.DTOs.Payment;
using CarRental.Application.Interfaces;
using CarRental.Infrastructure.Identity;
using CarRental.Infrastructure.Persistence;
using CarRental.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;
namespace CarRental.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<EmailSettingsDTO>(
    builder.Configuration.GetSection("EmailSettings"));

            // Database

            builder.Services.AddDbContext<CarRentalDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString(
                        "DefaultConnection")));

            // Redis Connection 
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration =
                    builder.Configuration.GetConnectionString("Redis");

                options.InstanceName = "CarRental:";
            });

            // Identity

            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<CarRentalDbContext>()
                .AddDefaultTokenProviders();


            // Authentication - JWT

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,

                            ValidateAudience = true,

                            ValidateLifetime = true,

                            ValidateIssuerSigningKey = true,

                            ValidIssuer =
                                builder.Configuration[
                                    "JwtSettings:Issuer"],

                            ValidAudience =
                                builder.Configuration[
                                    "JwtSettings:Audience"],

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        builder.Configuration[
                                            "JwtSettings:Key"]!))
                        };
                });
            //SignalR

            builder.Services.AddSignalR();

            //Car Images Options
            builder.Services.Configure<CarImagesOptions>(
    builder.Configuration.GetSection("CarImages"));

            builder.Services.AddHttpClient<ICarImageService, CarImageService>();

            //Stripe
            builder.Services.Configure<StripeSettingsDTO>(
    builder.Configuration.GetSection("Stripe"));

            // Dependency Injection

            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddScoped<ITokenService, TokenService>();

            builder.Services.AddScoped<IOTPService, OTPService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ICarService, CarService>();
            builder.Services.AddScoped<IBranchService, BranchService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            builder.Services.AddScoped<IConversationService, ConversationService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IPaginationService, PaginationService>();
            builder.Services.AddScoped<IFavoriteListService, FavoriteListService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<ICacheService, CacheService>();

            // Controllers

            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddMemoryCache();

            //Front connection
            //
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:5500",
                            "http://127.0.0.1:5500",
                            "http://localhost:5501",
                            "http://127.0.0.1:5501"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            // Swagger

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "Car Rental API",
                        Version = "v1",
                        Description =
                            "Car Rental System RESTful API"
                    });


                // JWT Authentication
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",

                        Type = SecuritySchemeType.Http,

                        Scheme = "Bearer",

                        BearerFormat = "JWT",

                        In = ParameterLocation.Header,

                        Description =
                            "Enter JWT token like: Bearer {your token}"
                    });


                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference =
                                    new OpenApiReference
                                    {
                                        Type =
                                            ReferenceType.SecurityScheme,

                                        Id = "Bearer"
                                    }
                            },
                            Array.Empty<string>()
                        }
                    });
            });


            // Build

            var app = builder.Build();


            // Seed Roles

            using (var scope =
                   app.Services.CreateScope())
            {
                var roleManager =
                    scope.ServiceProvider
                        .GetRequiredService<
                            RoleManager<IdentityRole>>();

                var userManager =
                    scope.ServiceProvider
                        .GetRequiredService<
                            UserManager<ApplicationUser>>();

                await IdentitySeeder.SeedRolesAsync(
                    roleManager);

                await IdentitySeeder.SeedSuperAdminAsync(
                    userManager,
                    builder.Configuration);
            }


            // HTTP Pipeline

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint(
                        "/swagger/v1/swagger.json",
                        "Car Rental API V1");

                    options.RoutePrefix = "swagger";
                });
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("FrontendPolicy");



            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();
            app.MapHub<CarRental.Infrastructure.Hubs.ChatHub>("/hubs/chat");
            app.Run();
        }
    }
}