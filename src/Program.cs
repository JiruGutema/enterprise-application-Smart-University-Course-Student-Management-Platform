using Microsoft.EntityFrameworkCore;
using SmartUniversity.Middlewares;
using SmartUniversity.Modules.Identity;
using SmartUniversity.Modules.Identity.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

//Users
builder.Services.AddDbContext<UserDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddUsersModule();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
