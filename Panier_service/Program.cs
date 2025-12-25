using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Panier_service.Data;
using Panier_service.Services;
using Panier_service.Services.Kafka;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(
        options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
        });

builder.Services.AddHttpClient<ProductService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHostedService<KafkaConsumerservice>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    options =>
    {
        // Here we mention the rules that UseAuthenticaton middleware have to check if the token is valid or not
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetValue<string>("AppSettings:Issuer"),
            ValidateAudience = true,
            ValidAudience = builder.Configuration.GetValue<string>("AppSettings:Audience"),
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("AppSettings:Token")!)),
            ValidateIssuerSigningKey = true
        };
        // N.B : So whene the user send a login request the middleware UseAuthentication, the client send the jwt 
        // token if his connected, now the middleware check if this token is valid, if yes he create
        // HttpContext.User with claims, now when we send request to an endpoint who have Authorize annotation 
        // the middleware UseAuthorization see if the HttpContext.User is filled, if no he return 401 status code 
        // Not athentified, and if he is filled and he doesnt have the role he return 403 status code, not 
        // authorized to this action 
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
