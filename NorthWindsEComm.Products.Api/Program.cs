using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.EntityFrameworkCore;
using NorthWindsEComm.CrudHelper;
using NorthWindsEComm.Products.Api;

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

builder.Services.AddTransient<ICrudManager<Product>, CrudManager<Product>>();
builder.Services.AddTransient<ICrudCacheAccess<Product>, RedisCacheHelper<Product>>();
builder.Services.AddTransient<ICrudDataAccess<Product>, ProductDataAccess>();

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
        async (ICrudManager<Product> manager, CancellationToken token) => await manager.GetAllAsync(token))
    .WithName("GetProducts");

productsGroupV1.MapGet("/{id:int}", async (int id, ICrudManager<Product> manager, CancellationToken token) =>
    {
        Product? product = await manager.GetByIdAsync(id, token);
        return product == null ? Results.NotFound() : Results.Ok(product);
    })
    .WithName("GetProductById");

app.Run();

