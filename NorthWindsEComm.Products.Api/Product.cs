using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using NorthWindsEComm.CrudHelper;

namespace NorthWindsEComm.Products.Api;

public record Product : IIdModel
{
    [IgnoreDataMember]
    [JsonIgnore]
    public int Id => ProductId;
    [Key] public int ProductId { get; set; }
    [Required] [MaxLength(40)] public string? ProductName { get; set; }
    public int SupplierId { get; set; }
    public int CategoryId { get; set; }
    [MaxLength(20)] public string? QuantityPerUnit { get; set; }
    public decimal UnitPrice { get; set; }
    public short UnitsInStock { get; set; }
    public short UnitsOnOrder { get; set; }
    public short ReorderLevel { get; set; }
    public bool Discontinued { get; set; }
}