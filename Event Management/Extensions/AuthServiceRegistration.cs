﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

namespace Event_Management.Extensions
{
    public static class AuthServiceRegistration
    {
        public static void AddAuthServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure JWT authentication
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option =>
                {
                    option.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };

                    option.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/userHub") || path.StartsWithSegments("/participantHub"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            // Configure authorization policies
            services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
                {
                    Description = "enter authorization token using bearer scheme (bearer {token})",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                option.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "Event-Management"; // Instance name for Redis cache (Name of your created container)
            });

            // CORS setup
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .WithExposedHeaders("x-requested-with")
                           .AllowCredentials();
                });
            });

            // Configure Web API with Newtonsoft.Json
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            services.AddHttpContextAccessor();
        }
    }
}
