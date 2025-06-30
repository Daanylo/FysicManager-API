using FysicManagerAPI;
using FysicManagerAPI.Data;
using FysicManagerAPI.Models;
using FysicManagerAPI.Middleware;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddOpenApi();

// Configure JSON options for proper DateTime handling
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Add session support for tracking user sessions
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", 
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Seed mock data
// Apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    // Apply any pending migrations
    db.Database.Migrate();
    
    // Seed the database with initial data
    DbSeeder.Seed(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Enable CORS
app.UseCors("AllowAllOrigins");

// Enable sessions
app.UseSession();

// Add access logging middleware
app.UseMiddleware<AccessLoggingMiddleware>();

app.UseHttpsRedirection();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();

public partial class Program { }