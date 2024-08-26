using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthWindsEComm.CrudHelper;
using NorthWindsEComm.Products.Api;
using ApiVersion = Asp.Versioning.ApiVersion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.ReportApiVersions = true;
    config.AssumeDefaultVersionWhenUnspecified = false;
    config.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(config=>{config.GroupNameFormat = "v'V'";});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<NorthWindsDbContext>("northWindsData");

builder.AddRedisClient("cache");

builder.Services.AddTransient<IProductManager, ProductManager>();
builder.Services.AddTransient<ICrudCacheAccess<Product>, RedisCacheHelper<Product>>();
builder.Services.AddTransient<IProductDataAccess, ProductDataAccess>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

ApiVersionSet apiVersionSet = app.NewApiVersionSet("products-api")
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

app.UseHttpsRedirection();

var productsGroup = app.MapGroup("/v{version:apiVersion}/products")
    .WithApiVersionSet(apiVersionSet)
    .WithOpenApi();

var productsGroupV1 = productsGroup.MapGroup("/")
    .MapToApiVersion(1);

productsGroupV1.MapGet("/",
        async (IProductManager manager, CancellationToken token, [FromQuery(Name = "categoryId")] int? categoryId = null) => 
        categoryId is null ? await manager.GetAllAsync(token) : await manager.GetProductsByCategoryIdAsync((int)categoryId, token))
    .WithName("GetProducts");

productsGroupV1.MapGet("/{id:int}", async (int id, IProductManager manager, CancellationToken token) =>
    {
        Product? product = await manager.GetByIdAsync(id, token);
        return product == null ? Results.NotFound() : Results.Ok(product);
    })
    .WithName("GetProductById");

app.Run();

