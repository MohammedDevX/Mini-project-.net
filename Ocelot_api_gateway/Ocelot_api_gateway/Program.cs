using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// builder.Configuration : l'objet qui contient touts les configurations de variables env et fichiers json etc ... 
// .AddJsonFile => on va ajouter un fichier json appeller ocelot.json à la configuration
// optionel: false => veut dire, si ce fichier n'existe pas le projet ne doit pas travailler donc ce fichiers pas 
// optionel 
// reloadOnChange => quand on changue dans ce fichier faire un auto reload 
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

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
        // Not athentified, and if he is filled and he doesnt have the role he return 403 status code Forbidden, not 
        // authorized to this action 
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
await app.UseOcelot();

app.MapGet("/", () => "Hello World!");

app.Run();
