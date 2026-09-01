using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PayTrack.Application;
using PayTrack.Application.Features.Auth.Commands;
using PayTrack.Domain.Entities;
using PayTrack.Infrastructure;
using PayTrack.Infrastructure.Data;
using System.Reflection;
using System.Text;

namespace PayTrack.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration["Cors:AllowedOrigins"]?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                ?? throw new InvalidOperationException("AllowedOrigins missing ");

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials();
                });
            });
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddApplication()
                .AddInfrastructure();
            return services;
        }
        public static IServiceCollection AddDatabaseAndIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            return services;
        }
        public static IServiceCollection AddJwtAuthentication(
     this IServiceCollection services,
     IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })

            // JWT AUTHENTICATION

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
                            configuration["Jwt:Issuer"],

                        ValidAudience =
                            configuration["Jwt:Audience"],

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    configuration["Jwt:Key"]!
                                )
                            ),

                        ClockSkew = TimeSpan.Zero
                    };
    
                // SIGNALR

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken =
                            context.Request.Query["access_token"];

                        var path =
                            context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments(
                                "/notificationHub"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            })

            // GOOGLE AUTHENTICATION

            .AddGoogle(
                GoogleDefaults.AuthenticationScheme,
                options =>
                {
                    options.ClientId =
                        configuration[
                            "Authentication:Google:ClientId"
                        ]!;

                    options.ClientSecret =
                        configuration[
                            "Authentication:Google:ClientSecret"
                        ]!;

                    options.CallbackPath =
                        "/signin-google";

                    options.SignInScheme =
                        IdentityConstants.ExternalScheme;
                }
            );

            return services;
        }
        public static IServiceCollection AddSwaggerWithAuth(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    Array.Empty<string>()
                }
            });
            });

            return services;
        }
        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            var assembly = typeof(RegisterCommandHandler).Assembly;

            try
            {
                services.AddMediatR(configuration =>
                {
                    configuration.RegisterServicesFromAssembly(assembly);
                });
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var loaderEx in ex.LoaderExceptions)
                {
                    Console.WriteLine(loaderEx.Message); // Inspect which type failed
                }
                throw;
            }

            return services;
        }


    }
}
