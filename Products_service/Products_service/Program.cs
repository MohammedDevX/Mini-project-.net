using Microsoft.EntityFrameworkCore;
using Products_service.Data;
using Products_service.Repositories.Category;
using Products_service.Repositories.Products;
using Products_service.Services;
using Products_service.Services.Caching;
using Products_service.Services.Kafka;
using Products_service.Services.SaveFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
});

builder.Services.AddHttpClient<UserService>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Catalogue";
});
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<IMinioService, MinioService>();
builder.Services.AddSingleton<IKafkaService, KakfaService>();
builder.Services.AddScoped<ICategoryR, CategoryR>();
builder.Services.AddScoped<IProduitR, ProduitR>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
