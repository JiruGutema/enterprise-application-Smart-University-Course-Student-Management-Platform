using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using dotenv.net;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using  Swashbuckle.AspNetCore;
using Microsoft.OpenApi;
using Microsoft.IdentityModel.Tokens;
using SmartUniversity.Modules.Enrollment.Api;
using SmartUniversity.Modules.Enrollment.Application;
using SmartUniversity.Modules.Enrollment.Application.Commands;
using SmartUniversity.Modules.Enrollment.Domain.Repositories;
using SmartUniversity.Modules.Enrollment.Infrastructure.Outbox;
using SmartUniversity.Modules.Enrollment.Infrastructure.Persistence;
using SmartUniversity.Modules.Enrollment.Infrastructure.Repositories;
using SmartUniversity.Modules.Identity;
using SmartUniversity.Modules.AI;
using SmartUniversity.Modules.GradingAndAssessment;
using SmartUniversity.Modules.Identity.Infrastructure.Persistence;
using SmartUniversity.Modules.Notification.Infrastructure.Persistence;
using SmartUniversity.Modules.AI.Infrastructure.Persistence;
using SmartUniversity.Shared.Kernel.Infrastructure.Messaging;
using SmartUniversity.Shared.Kernel.Interface;
using SmartUniversity.Shared.Middleware;
using SmartUniversity.Modules.Content.Api;
using SmartUniversity.Modules.Content.Application.Services;
using SmartUniversity.Modules.Content.Domain.Repositories;
using SmartUniversity.Modules.Content.Infrastructure.Persistence;
using Quartz;
using SmartUniversity.Modules.Identity.Infrastructure.Outbox;
using SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Outbox;

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


builder.Services.AddSwaggerGen(c =>
{

// since we don't this we just use the inline commenting 
    // Optional: XML comments
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, "YourApi.xml");
    // c.IncludeXmlComments(xmlPath);
 
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter your token (without 'Bearer ' prefix).",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",   
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document), 
            new List<string>()
        }
    });
});

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

builder.Services.AddDbContext<UserDbContext>(options => 
    options.UseNpgsql(connectionString)
           .AddInterceptors(new IdentityOutboxInterceptor()));

builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

builder.Services.AddDbContext<AIDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDbContext<ContentDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<MaterialService>();

// RabbitMQ Configuration
builder.Services.AddSingleton(
    new RabbitMqConnection(
        host: builder.Configuration["RabbitMQ:Host"]!,
        username: builder.Configuration["RabbitMQ:Username"]!,
        password: builder.Configuration["RabbitMQ:Password"]!
    )
);

builder.Services.AddSingleton<IEventBus, RabbitMqEventBus>();

// Enrollment DbContext
builder.Services.AddDbContext<EnrollmentDbContext>(options =>
{
    options.UseNpgsql(connectionString).AddInterceptors(new OutboxInterceptor());
});

builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddMediatR(typeof(Program).Assembly);

// Register Identity Outbox Publisher
builder.Services.AddScoped<IdentityOutboxPublisher>();
builder.Services.AddScoped<GradingOutboxPublisher>();
builder.Services.AddScoped<EnrollmentOutboxPublisher>();


// Quartz Configuration
builder.Services.AddQuartz(q =>
{
    var identityJobKey = new JobKey("IdentityOutboxPublishJob");
    q.AddJob<IdentityOutboxPublishJob>(opts => opts.WithIdentity(identityJobKey));
    q.AddTrigger(opts => opts
        .ForJob(identityJobKey)
        .WithIdentity("IdentityOutboxPublishJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(10)
            .RepeatForever()));

    var gradingJobKey = new JobKey("GradingOutboxPublishJob");
    q.AddJob<GradingOutboxPublishJob>(opts => opts.WithIdentity(gradingJobKey));
    q.AddTrigger(opts => opts
        .ForJob(gradingJobKey)
        .WithIdentity("GradingOutboxPublishJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(10)
            .RepeatForever()));

    var enrollmentJobKey = new JobKey("EnrollmentOutboxPublishJob");

    q.AddJob<EnrollmentOutboxPublishJob>(opts => opts.WithIdentity(enrollmentJobKey));

    q.AddTrigger(opts => opts
        .ForJob(enrollmentJobKey)
        .WithIdentity("EnrollmentOutboxPublishJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(10) 
            .RepeatForever()));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Modules
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddNotificationModule(builder.Configuration);
builder.Services.AddAIModule(builder.Configuration);
builder.Services.AddGradingAndAssessmentModule(builder.Configuration);

var app = builder.Build();

// register event subscriptions
app.SubscribeNotificationEvents();
// app.SubscribeEnrollmentEvents();

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
app.MapContentEndpoints();

app.Run();
