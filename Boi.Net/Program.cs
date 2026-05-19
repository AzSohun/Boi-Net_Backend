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
using Stripe;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(cfg => { }, typeof(Program));
builder.Services.AddOpenApi();


builder.Services.AddDbContext<BoiNetDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        ClockSkew = TimeSpan.Zero // When the token exipires it makes sure the authorization will invalid. 
    };
});

// Mapping from AppSettings to CloudinarySetting
//builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings")); <--- Previously I Wrote this
builder.Services.Configure<Boi.Net.Settings.CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));


// Redis Registration
builder.Services.AddStackExchangeRedisCache(options => {

    options.Configuration = builder.Configuration["ConnectionStrings:RedisConnection"];
    options.InstanceName = "BoiNet_";
});


// Add the Photo Services
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<UserService>();

// Stripe Service
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];


// Global Exceptions Handler Services
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://boi-net-frontend.vercel.app")
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<BoiNetDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();
