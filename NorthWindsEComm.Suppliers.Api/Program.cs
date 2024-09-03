using System.Diagnostics;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NorthWindsEComm.CrudHelper;
using NorthWindsEComm.CrudHelper.Base;
using NorthWindsEComm.CrudHelper.Cache;
using NorthWindsEComm.CrudHelper.Messaging;
using NorthWindsEComm.Suppliers.Api;
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
    options.SwaggerDoc("v1", new OpenApiInfo { Title = $"Suppliers v1", Version = "v1" });
    var filePath = Path.Combine(AppContext.BaseDirectory, $"NorthWindsEComm.Suppliers.Api.xml");
    options.IncludeXmlComments(filePath);
});
builder.AddServiceDefaults();

builder.AddSqlServerDbContext<SupplierContext>("northWindsData");

builder.AddRedisClient("cache");

builder.Services.AddTransient<IKafkaHelper<Supplier>, KafkaHelper<Supplier>>();
builder.Services.AddTransient<ICrudManager<Supplier>, CrudManager<Supplier>>();
builder.Services.AddTransient<ICrudCacheAccess<Supplier>, RedisCacheHelper<Supplier>>();
builder.Services.AddTransient<ICrudDataAccess<Supplier>, SupplierDataAccess>();
builder.Services.AddSingleton<CacheHitMetrics>();

builder.AddKafkaProducer<string, string>("messaging");

var app = builder.Build();
app.MapDefaultEndpoints();

ApiVersionSet apiVersionSet = app.NewApiVersionSet("suppliers-api")
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

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
            var name = $"Suppliers {description.GroupName}";
            options.SwaggerEndpoint(url, name);
        }
    });
}

app.UseHttpsRedirection();

var supplierGroup = app.MapGroup("/v{version:apiVersion}/suppliers")
    .WithApiVersionSet(apiVersionSet)
    .WithOpenApi();

var supplierGroupV1 = supplierGroup.MapGroup("/")
    .MapToApiVersion(1);

supplierGroupV1.MapGet("/", async (ICrudManager<Supplier> manager, CancellationToken token) => await manager.GetAllAsync(token))
    .WithName("GetSuppliers");

supplierGroupV1.MapPost("/",
        async ([FromBody] Supplier supplier, ICrudManager<Supplier> manager, CancellationToken token) => await manager.CreateAsync(supplier, token))
    .WithName("CreateSupplier");

supplierGroupV1.MapGet("/{id:int}", async (int id, ICrudManager<Supplier> manager, CancellationToken token) =>
    {
        Supplier? supplier = await manager.GetByIdAsync(id, token);
        return supplier == null ? Results.NotFound() : Results.Ok(supplier);
    })
    .WithName("GetSupplierById");

supplierGroupV1.MapPut("/{id:int}", async ([FromRoute] int id, [FromBody]Supplier supplier, ICrudManager<Supplier> manager, CancellationToken token) =>
    {
        Supplier? response = await manager.UpdateAsync(id, supplier, token);
        return response == null ? Results.NotFound() : Results.Ok(response);
    })
    .WithName("UpdateSupplierById");

app.Run();