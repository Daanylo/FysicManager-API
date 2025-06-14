using FysicManagerAPI;
using FysicManagerAPI.Data;
using FysicManagerAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("FysicManagerDb"));
builder.Services.AddOpenApi();
builder.Services.AddControllers();
var app = builder.Build();

// Seed mock data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllerRoute(
    name: "patient",
    pattern: "api/patient/{id?}",
    defaults: new { controller = "Patient", action = "GetPatient" }
);

app.Run();

public partial class Program { }