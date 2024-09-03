using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using NorthWindsEComm.CrudHelper;
using NorthWindsEComm.CrudHelper.Base;

namespace NorthWindsEComm.Products.Api;

/// <summary>
/// Represents a product.
/// </summary>
public record Product : IIdModel
{
    /// Represents the unique identifier of a Product.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public int Id => ProductId;

    /// <summary>
    /// Represents the unique identifier of a Product.
    /// </summary>
    [Key]
    public int ProductId { get; set; }

    /// <summary>
    /// Represents a product.
    /// </summary>
    [Required]
    [MaxLength(40)]
    public string? ProductName { get; set; }

    /// <summary>
    /// Represents the identifier of the supplier for a product.
    /// </summary>
    public int SupplierId { get; set; }

    /// <summary>
    /// Represents a unique identifier for a category of products.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the quantity per unit of the product.
    /// </summary>
    [MaxLength(20)]
    public string? QuantityPerUnit { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Represents the number of units in stock for a product.
    /// </summary>
    public short UnitsInStock { get; set; }

    /// <summary>
    /// Gets or sets the number of units currently on order for the product.
    /// </summary>
    public short UnitsOnOrder { get; set; }

    /// <summary>
    /// Gets or sets the reorder level of a product.
    /// </summary>
    public short ReorderLevel { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is discontinued.
    /// </summary>
    public bool Discontinued { get; set; }
}