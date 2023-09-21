using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SalesRepo.Data.Models;
using SalesRepo.Domain.Exceptions;
using SalesRepo.Domain.Interface;
using SalesRepo.Domain.Models.Request;
using SalesRepo.Domain.Service;
using SalesRepo.UnitTests.Validation;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace SalesRepo.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SalesRepo API",
                Version = "v1"
            });
            c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                }] = Array.Empty<string>(),
            });
        });

        builder.Services.AddResponseCompression();

        builder.Services.AddDbContext<SalesRepoContext>(a =>
        {
            a.UseSqlServer(builder.Configuration["SalesRepoApi:DbConnectionString"], b => b.EnableRetryOnFailure());

        }, ServiceLifetime.Transient, ServiceLifetime.Singleton);

        builder.Services.AddSingleton<Func<SalesRepoContext>>(a => () => a.GetRequiredService<SalesRepoContext>());
        builder.Services.AddSingleton<IOrderService, OrderService>();
        builder.Services.AddSingleton<IProductService, ProductService>();
        builder.Services.AddSingleton<ICustomerService, CustomerService>();
        builder.Services.AddSingleton<IValidator<CreateCustomerRequest>, CreateCustomerValidator>();
        builder.Services.AddSingleton<IValidator<CreateProductRequest>, CreateProductValidator>();
        builder.Services.AddSingleton<IValidator<UpdateCustomerRequest>, UpdateCustomerValidator>();
        builder.Services.AddSingleton<IValidator<UpdateProductRequest>, UpdateProductValidator>();
        builder.Services.AddSingleton<IValidator<UpdateOrderRequest>, UpdateOrderValidator>();
        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        builder.Services.AddHealthChecks();

        var app = builder.Build();

        app.UseHttpLogging();
        app.UseResponseCompression();

        app.UseExceptionHandler(appBuilder =>
        {
            appBuilder.Run(async httpContext =>
            {
                var exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
                var exception = exceptionDetails?.Error;

                var statusCode = HttpStatusCode.InternalServerError; 
                string message = "Unhandled Exception";

                switch (exception)
                {
                    case EntityNotFoundException:
                        statusCode = HttpStatusCode.NotFound;
                        message = $"Not Found: {exception.Message}";
                        break;
                    case ValidationException:
                        statusCode = HttpStatusCode.BadRequest;
                        message = $"Bad Request: {exception.Message}";
                        break;
                    case InvalidUpdateException:
                        statusCode = HttpStatusCode.UnprocessableEntity;
                        message = $"Invalid Update Request: {exception.Message}";
                        break;
                    default:
                        appBuilder.ApplicationServices.GetRequiredService<ILogger<Program>>().LogError(exception, "Unhandled exception");
                        break;
                }

                httpContext.Response.StatusCode = (int)statusCode;
                httpContext.Response.ContentType = Text.Plain;

                await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(message));
            });
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/health").AllowAnonymous();

        app.Run();
    }
}