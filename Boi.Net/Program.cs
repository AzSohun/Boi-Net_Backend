using Boi.Net.Data;
using Boi.Net.Exceptions;
using Boi.Net.Model;
using Boi.Net.Services;
using Boi.Net.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(cfg => { }, typeof(Program));
builder.Services.AddOpenApi();


builder.Services.AddDbContext<BoiNetDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = true;

    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<BoiNetDbContext>()
.AddDefaultTokenProviders();


// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["jwt:Audience"],

        ClockSkew = TimeSpan.Zero // When the token exipires it makes sure the authorization will invalid. 
    };
});

// Mapping from AppSettings to CloudinarySetting
//builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings")); <--- Previously I Wrote this
builder.Services.Configure<Boi.Net.Settings.CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
// Add the Photo Services
builder.Services.AddScoped<IPhotoService, PhotoService>();

builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<AuthService>();



// Global Exceptions Handler Services
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
