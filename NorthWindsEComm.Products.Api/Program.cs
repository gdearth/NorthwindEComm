using System.Threading;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NorthWindsEComm.CrudHelper;
using NorthWindsEComm.Products.Api;
using ApiVersion = Asp.Versioning.ApiVersion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
}).AddApiExplorer(config =>
{
    config.GroupNameFormat = "'v'VVV";
    config.SubstituteApiVersionInUrl = true;
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = $"Products v1", Version = "v1" });
    var filePath = Path.Combine(AppContext.BaseDirectory, $"NorthWindsEComm.Products.Api.xml");
    options.IncludeXmlComments(filePath);
});

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<NorthWindsDbContext>("northWindsData");

builder.AddRedisClient("cache");

builder.Services.AddTransient<IProductManager, ProductManager>();
builder.Services.AddTransient<ICrudCacheAccess<Product>, RedisCacheHelper<Product>>();
builder.Services.AddTransient<IProductDataAccess, ProductDataAccess>();
builder.Services.AddTransient<StartupService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = $"Products {description.GroupName}";
            options.SwaggerEndpoint(url, name);
        }
    });
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

productsGroupV1.MapPost("/",
        async ([FromBody] Product product, IProductManager manager, CancellationToken token) => await manager.CreateAsync(product, token))
    .WithName("CreateProducts");

productsGroupV1.MapGet("/{id:int}", async (int id, IProductManager manager, CancellationToken token) =>
    {
        Product? product = await manager.GetByIdAsync(id, token);
        return product == null ? Results.NotFound() : Results.Ok(product);
    })
    .WithName("GetProductById");

productsGroupV1.MapPut("/{id:int}", async ([FromRoute] int id, [FromBody]Product product, IProductManager manager, CancellationToken token) =>
    {
        Product? response = await manager.UpdateAsync(id, product, token);
        return response == null ? Results.NotFound() : Results.Ok(response);
    })
    .WithName("UpdateProductById");

app.Run();