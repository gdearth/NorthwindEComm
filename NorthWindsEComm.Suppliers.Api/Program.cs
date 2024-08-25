using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.EntityFrameworkCore;
using NorthWindsEComm.CrudHelper;
using NorthWindsEComm.Suppliers.Api;

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

builder.AddSqlServerDbContext<SupplierContext>("northWindsData");

builder.AddRedisClient("cache");

builder.Services.AddTransient<ICrudManager<Supplier>, CrudManager<Supplier>>();
builder.Services.AddTransient<ICrudCacheAccess<Supplier>, RedisCacheHelper<Supplier>>();
builder.Services.AddTransient<ICrudDataAccess<Supplier>, SupplierDataAccess>();

var app = builder.Build();
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

ApiVersionSet apiVersionSet = app.NewApiVersionSet("suppliers-api")
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

app.UseHttpsRedirection();

var supplierGroup = app.MapGroup("/v{version:apiVersion}/suppliers")
    .WithApiVersionSet(apiVersionSet)
    .WithOpenApi();

var supplierGroupV1 = supplierGroup.MapGroup("/")
    .MapToApiVersion(1);

supplierGroupV1.MapGet("/", async (ICrudManager<Supplier> manager, CancellationToken token) => await manager.GetAllAsync(token))
    .WithName("GetSuppliers");

supplierGroupV1.MapGet("/{id:int}", async (int id, ICrudManager<Supplier> manager, CancellationToken token) =>
    {
        Supplier? supplier = await manager.GetByIdAsync(id, token);
        return supplier == null ? Results.NotFound() : Results.Ok(supplier);
    })
    .WithName("GetSupplierById");

app.Run();