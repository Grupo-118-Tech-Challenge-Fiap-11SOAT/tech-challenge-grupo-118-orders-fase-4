using System.Reflection;
using System.Text.Json.Serialization;
using Application.Products;
using Domain.Products.Ports.In;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Orders.API.Handlers;
using Domain.Order.Ports.In;
using Domain.Order.Ports.Out;
using Application.Order;
using DefaultNamespace;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;
using Refit;
using Domain.Order.Services.Interfaces;
using Domain.Order.Services;
using Infra.Database.Postgres;
using Infra.Database.Postgres.Order.Repository;

namespace Orders.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddExceptionHandler<CustomExceptionHandler>();
        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddControllers().AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                }
            )
            .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    var compiledErrors = context.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)
                        .ToArray();

                    var response = new ProblemDetails
                    {
                        Type = "",
                        Title = "One or more model validation errors occurred.",
                        Detail = string.Join(" || ", compiledErrors)
                    };

                    return new BadRequestObjectResult(response);
                };
            });

        builder.Services.AddControllers(options => { options.SuppressAsyncSuffixInActionNames = false; });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            // using System.Reflection;
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        //Uso via variavel de ambiente (Double underscore para representar o nível): ConnectionStrings__DefaultConnection

        //builder.Services.Configure<MercadoPagoOptions>(builder.Configuration.GetSection("MercadoPago"));

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        //TODO: Insert Dependency Injections implementation
        builder.Services.AddTransient<IProductManager, ProductManager>();
       
        builder.Services.AddTransient<IOrderRepository, OrderRepository>();
        builder.Services.AddTransient<IOrderManager, OrderManager>();
        builder.Services.AddTransient<IOrderService, OrderService>();

        builder.Services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Tech Challenge - Fast Food API - Orders",
                Version = "v1",
                Description = "API para gerenciamento de pedidos para lanchonete.",
                Contact = new OpenApiContact
                {
                    Name = "Grupo 118 - Sabrina Cardoso | Tiago Koch | Tiago Oliveira | Túlio Rezende | Vinícius Nunes",
                    Url = new Uri(
                        "https://github.com/Grupo-118-Tech-Challenge-Fiap-11SOAT/tech-challenge-grupo-118-fase-4-orders")
                }
            });

            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                              "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                              "Example: \"Bearer 12345abcdef\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

        });

        // Configuração JWT
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey =
                        new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

        builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

        var app = builder.Build();
        //Clean the Standard Exception handlers to a more custom return
        app.UseExceptionHandler(_ => { });

        // Execute migrations automatically on app startup
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI(s =>
        {
            s.SwaggerEndpoint("../swagger/v1/swagger.json", "Tech Challenge - Fast Food API");
            s.RoutePrefix = string.Empty;
            s.DocumentTitle = "Tech Challenge - Fast Food API | Swagger";
        });

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapHealthChecks("/healthz");

        app.Run();
    }
}