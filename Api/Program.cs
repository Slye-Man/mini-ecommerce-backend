using Api.Middleware;
using Microsoft.OpenApi.Models;
using Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register AuthService
builder.Services.AddScoped<IAuthService, AuthService>();

// Implementing CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Implementing UserService
builder.Services.AddScoped<IUserService, UserService>();

// Implementing ProductService
builder.Services.AddScoped<IProductService, ProductService>();

// Implementing CartService
builder.Services.AddScoped<ICartService, CartService>();

// Implementing OrderService
builder.Services.AddScoped<IOrderService, OrderService>();

// Implementing SessionService
builder.Services.AddScoped<ISessionService, SessionService>();

var app = builder.Build();

// Creating database if it does not exist
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

// Seed products
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    ProductSeeder.SeedProducts(dbContext);
}

app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCustomAuthentication();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("v1/swagger.json", "My API V1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();