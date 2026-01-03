using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartUniversity.Modules.Identity;
using SmartUniversity.Modules.Identity.Infrastructure.Persistence;
using SmartUniversity.Modules.Notification.Infrastructure.Persistence;
using SmartUniversity.Shared.Kernel.Interface;
using SmartUniversity.Shared.Kernel.Service;
using SmartUniversity.Shared.Middleware;

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
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Shared event bus
builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();

// Modules
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddNotificationModule(builder.Configuration);

var app = builder.Build();

// register event subscriptions
app.SubscribeNotificationEvents();

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
