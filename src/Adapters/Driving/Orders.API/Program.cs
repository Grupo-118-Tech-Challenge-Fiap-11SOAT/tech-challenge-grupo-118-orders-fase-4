using System.Reflection;
using Application.Order;
using Domain.Order.Ports.In;
using Domain.Order.Ports.Out;
using Domain.Payment.Ports.Out;
using Domain.Products.Ports.In;
using External.Payments.API.Payment;
using External.Products.API.ProductsManagerAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Infra.Database.SqlServer;
using Infra.Database.SqlServer.Order.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

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
                             System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
                };
            });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
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

builder.Services.AddScoped<IOrderManager, OrderManager>();
builder.Services.AddHttpClient<IProductManager, ProductManager>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var options = configuration.GetSection("ProductsApi");
    
    client.BaseAddress = new Uri(options.GetValue<string>("BaseUrl"));
});
builder.Services.AddHttpClient<IPaymentManager, PaymentManager>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var options = configuration.GetSection("PaymentsApi");
    
    client.BaseAddress = new Uri(options.GetValue<string>("BaseUrl"));
});
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

var app = builder.Build();

app.UseExceptionHandler(_ => { });

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("../swagger/v1/swagger.json", "Tech Challenge - Fast Food - Orders API");
    s.RoutePrefix = string.Empty;
    s.DocumentTitle = "Tech Challenge - Orders API | Swagger";
});

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();    

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/healthz");
app.Run();