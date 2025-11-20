using GerenciadorDeProdutos.Models;
using GerenciadorDeProdutos.Repositories;
using GerenciadorDeProdutos.Services;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GerenciadorDeProdutos.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// MONGO CONFIG
// -----------------------------
var mongoConn = builder.Configuration.GetConnectionString("Mongo");
var mongoDbName = builder.Configuration.GetValue<string>("MongoDatabase") ?? "projetoc216";

var client = new MongoClient(mongoConn);
var database = client.GetDatabase(mongoDbName);

builder.Services.AddSingleton(database);

builder.Services.AddSingleton<IMongoRepository<User>>(sp => new MongoRepository<User>(database, "users"));
builder.Services.AddSingleton<IMongoRepository<Product>>(sp => new MongoRepository<Product>(database, "products"));
builder.Services.AddSingleton<IMongoRepository<Order>>(sp => new MongoRepository<Order>(database, "orders"));

// -----------------------------
// JWT CONFIG
// -----------------------------
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

// -----------------------------
// SERVICES
// -----------------------------
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<IMongoRepository<AuditLog>>(sp => 
    new MongoRepository<AuditLog>(database, "auditLogs"));

builder.Services.AddSingleton<AuditService>();


// -----------------------------
// CONTROLLERS / SWAGGER / CORS
// -----------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT: Bearer {seu_token}",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// -----------------------------
// BUILD
// -----------------------------
var app = builder.Build();
app.UseMiddleware<AuditMiddleware>();


app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// IMPORTANTE → A ordem importa
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
