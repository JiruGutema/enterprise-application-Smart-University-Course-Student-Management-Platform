using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartUniversity.Shared.Middleware;
using SmartUniversity.Modules.Identity;
using SmartUniversity.Modules.Identity.Infrastructure.Persistence;

using SmartUniversity.Modules.Enrollment.Api;
using SmartUniversity.Modules.Enrollment.Application;
using SmartUniversity.Modules.Enrollment.Application.Commands;
using SmartUniversity.Modules.Enrollment.Domain.Repositories;
using SmartUniversity.Modules.Enrollment.Infrastructure.Persistence;
using SmartUniversity.Modules.Enrollment.Infrastructure.Repositories;
using SmartUniversity.Modules.Enrollment.Infrastructure.Outbox;
using MediatR;


var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    DotEnv.Load();
}

// Make env vars available to IConfiguration
builder.Configuration.AddEnvironmentVariables();

// Read connection string from .env
var connectionString = builder.Configuration.GetConnectionString("Default");
var jwtSecret = builder.Configuration["JWT:Secret"];
var jwtissuer = builder.Configuration["JWT:Issuer"];
var jwtaudience = builder.Configuration["JWT:Audience"];
builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
builder.Services.AddSwaggerGen(c =>
{
    c.IncludeXmlComments(xmlPath);
});

//Athentication sevices
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtissuer,
            ValidAudience = jwtaudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!)),
            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["accessToken"];
                return Task.CompletedTask;
            },
        };
    });
builder.Services.AddAuthorization();

// Database
builder.Services.AddDbContext<UserDbContext>(options => options.UseNpgsql(connectionString));


// Enrollment DbContext
builder.Services.AddDbContext<EnrollmentDbContext>(options =>
{
    options.UseNpgsql(connectionString)
           .AddInterceptors(new OutboxInterceptor());
});

builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddMediatR(typeof(EnrollStudentCommand).Assembly);



// Modules
builder.Services.AddIdentityModule(builder.Configuration);

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseWelcomePage();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
