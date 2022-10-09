using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RestaurantDAL;
using RestaurantDAL.EntityFrameworkUtility;
using RestaurantWebAPI.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//code to add Swagger/OpenAPI documentation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Restaurant Management API",
        Version = "v1",
        Description = "Restaurant Management API Swagger Documentation"
    });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
//code to add swagger documentation

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddContext();

builder.Services.AddDbContext<RestaurantManagementContext>(options => options.UseSqlServer(Common.GetConnectionString()));

//Project Dependancy resolver
builder.Services.AddDAL();
builder.Services.AddBLL();
//Project Dependancy resolver

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
