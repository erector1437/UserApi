using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UserApi.Data;
using UserApi.Models;
using UserApi.Services;
using UserApi.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<UserContext>(options =>
    options.UseInMemoryDatabase("UserDb"));
builder.Services.AddScoped<IUserService, UserService>(); // Ensure this line is present
builder.Services.AddScoped<IValidator<User>, UserValidator>();
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Run();
