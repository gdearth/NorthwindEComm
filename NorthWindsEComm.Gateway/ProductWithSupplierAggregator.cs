using System.Text.Json;
using NorthWindsEComm.Products.Api;
using NorthWindsEComm.Suppliers.Api;
using Ocelot.Middleware;
using Ocelot.Multiplexer;

namespace NorthWindsEComm.Gateway;

public class ProductWithSupplierAggregator : IDefinedAggregator
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductWithSupplierAggregator> _logger;

    public ProductWithSupplierAggregator(HttpClient httpClient, ILogger<ProductWithSupplierAggregator> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _logger.LogInformation("ProductWithSupplierAggregator has been initialized.");
    }

    public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
    {
        _logger.LogInformation("ProductWithSupplierAggregator has started.");
        // Get the product response
        string productResponse = await responses[0].Items.DownstreamResponse().Content.ReadAsStringAsync();
        ProductWithSupplier? product = (ProductWithSupplier?)JsonSerializer.Deserialize<Product>(productResponse);

        if (product is not null)
        {
            // Use the supplierId from the product response to call the Suppliers API
            string supplierResponse =
                await _httpClient.GetStringAsync($"http://localhost:5105/api/suppliers/{product.SupplierId}");
            Supplier? supplier = JsonSerializer.Deserialize<Supplier>(supplierResponse);

            if (supplier is not null)
            {
                // Combine the product and supplier data
                product.Supplier = supplier;
            }
        }

        // Serialize the combined data
        var aggregatedResult = JsonSerializer.Serialize(product);

        // Create the HTTP response
        var httpResponse = new HttpResponseMessage
        {
            Content = new StringContent(aggregatedResult)
        };

        _logger.LogInformation("ProductWithSupplierAggregator has finished.");
        return new DownstreamResponse(httpResponse);
    }
}

public record ProductWithSupplier : Product
{
    public Supplier Supplier { get; set; } = new();
}