using System.Reflection;
using Aspire.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("password", true);

var sql = builder.AddSqlServer("northWindsDb", password)
    .WithBindMount("./sqlserverconfig", "/usr/config")
    .WithBindMount("./sqlserverscripts", "/docker-entrypoint-initdb.d")
    .WithEntrypoint("/usr/config/entrypoint.sh").AddDatabase("northWindsData", "NorthWinds");

var redis = builder.AddRedis("cache");

var kafka = builder.AddKafka("messaging")
    .WithKafkaUI();

var productsApi = builder.AddProject<NorthWindsEComm_Products_Api>("products-api")
    .WithReference(sql)
    .WithReference(redis);

var suppliersApi = builder.AddProject<NorthWindsEComm_Suppliers_Api>("suppliers-api")
    .WithReference(sql)
    .WithReference(redis);

var categoriesApi = builder.AddProject<NorthWindsEComm_Categories_Api>("categories-api")
    .WithReference(sql)
    .WithReference(redis);

var gateway = builder.AddProject<NorthWindsEComm_Gateway>("gateway")
    .WithReference(productsApi)
    .WithReference(suppliersApi)
    .WithReference(categoriesApi)
    .WithExternalHttpEndpoints();

builder.Build().Run();
