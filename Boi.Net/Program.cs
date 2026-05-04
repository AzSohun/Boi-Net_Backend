using Boi.Net.Data;
using Boi.Net.Exceptions;
using Boi.Net.Services;
using Boi.Net.Settings;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(cfg => { }, typeof(Program));
builder.Services.AddOpenApi();

builder.Services.AddDbContext<BoiNetDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));


// Mapping from AppSettings to CloudinarySetting
builder.Services.Configure<CloudinarySetting>(builder.Configuration.GetSection("CloudinarySetting"));
// Add the Photo Services
builder.Services.AddScoped<IPhotoService, PhotoService>();

builder.Services.AddScoped<BookService>();



// Global Exceptions Handler Services
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();
