using System.Configuration;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service_Project.Context;
using Service_Project.Repositories;

var builder = WebApplication.CreateBuilder(args);
var corsPolicy = "CorsPolicy";

// Add services to the container.

// Authentication
builder.Services.AddAuthentication(x =>
{
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtSettings:SecretKey").Value!))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Set Database connection strings
builder.Services.AddDbContext<ProjectDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresSQL_DB")));

builder.Services.AddScoped<IBlobRepository>(provider => 
    new BlobRepository(builder.Configuration.GetConnectionString("AzureBlobStorage")));

// Services
builder.Services.AddAutoMapper(typeof(ProjectMappingProfile));  // Register AutoMapper profile

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

// CORS
builder.Services.AddCors(options =>
    options.AddPolicy(corsPolicy, policy => policy
        .WithOrigins(builder.Configuration.GetSection("AppSettings:AllowedOrigins").Get<string[]>()!)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(corsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
