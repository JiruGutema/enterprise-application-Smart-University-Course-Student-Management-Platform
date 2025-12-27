using dotenv.net;
using Microsoft.EntityFrameworkCore;
using SmartUniversity.Middlewares;
using SmartUniversity.Modules.Identity;
using SmartUniversity.Modules.Identity.Application.Interfaces;
using SmartUniversity.Modules.Identity.Infrastructure.Persistence;
using SmartUniversity.Modules.Identity.Infrastructure.Security;
using SmartUniversity.Modules.Identity.Domain.Repository;
using SmartUniversity.Modules.Identity.Application.Services;


var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    DotEnv.Load();
}

// Make env vars available to IConfiguration
builder.Configuration.AddEnvironmentVariables();

// Read connection string
var connectionString = builder.Configuration.GetConnectionString("Default");
var jwtSecret = builder.Configuration["JWT:Secret"];

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddSingleton<IJwtService>(new JwtService(jwtSecret));

// Database
builder.Services.AddDbContext<UserDbContext>(options => options.UseNpgsql(connectionString));

// Modules
builder.Services.AddUsersModule();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
