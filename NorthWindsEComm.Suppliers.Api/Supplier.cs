using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using NorthWindsEComm.CrudHelper;
using NorthWindsEComm.CrudHelper.Base;

namespace NorthWindsEComm.Suppliers.Api;

/// Represents a supplier.
/// This class represents a supplier in the system. It contains properties that describe various details
/// of the supplier such as the supplier ID, company name, contact name, contact title, address, city,
/// region, postal code, country, phone, fax, and homepage.
/// @implements IIdModel
/// @property {number} SupplierId - The ID of the supplier.
/// @property {string} CompanyName - The name of the company.
/// @property {string} [ContactName] - The name of the contact person.
/// @property {string} [ContactTitle] - The title of the contact person.
/// @property {string} [Address] - The address of the supplier.
/// @property {string} [City] - The city of the supplier's location.
/// @property {string} [Region] - The region of the supplier's location.
/// @property {string} [PostalCode] - The postal code of the supplier's location.
/// @property {string} [Country] - The country of the supplier's location.
/// @property {string} [Phone] - The contact phone number of the supplier.
/// @property {string} [Fax] - The fax number of the supplier.
/// @property {string} [HomePage] - The homepage URL of the supplier.
/// /
public record Supplier : IIdModel
{
    /// Gets the unique identifier of the supplier.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public int Id => SupplierId;

    /// <summary>
    /// Gets or sets the ID of the supplier.
    /// </summary>
    /// <remarks>
    /// The SupplierId property represents the unique identifier for a supplier.
    /// </remarks>
    [Key]
    public int SupplierId { get; set; }

    /// <summary>
    /// Gets or sets the name of the company.
    /// </summary>
    /// <remarks>
    /// This property specifies the name of the company associated with the supplier.
    /// </remarks>
    [Required]
    [MaxLength(40)]
    public string CompanyName { get; set; }

    /// Gets or sets the contact name of the supplier.
    /// </summary>
    /// <remarks>
    /// This property represents the contact name of the supplier.
    /// </remarks>
    [MaxLength(30)]
    public string? ContactName { get; set; }

    /// <summary>
    /// Gets or sets the contact title of the supplier.
    /// </summary>
    [MaxLength(30)]
    public string? ContactTitle { get; set; }

    /// <summary>
    /// Represents the address of a supplier.
    /// </summary>
    [MaxLength(60)]
    public string? Address { get; set; }

    /// Gets or sets the city of the supplier's address.
    /// </summary>
    /// <remarks>
    /// This property specifies the city of the supplier's address.
    /// </remarks>
    [MaxLength(15)]
    public string? City { get; set; }

    /// <summary>
    /// Represents the region where the supplier is located.
    /// </summary>
    [MaxLength(15)]
    public string? Region { get; set; }

    /// <summary>
    /// Gets or sets the postal code of the supplier's address.
    /// </summary>
    [MaxLength(10)]
    public string? PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the country of the supplier.
    /// </summary>
    [MaxLength(15)]
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the supplier.
    /// </summary>
    [MaxLength(24)]
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the fax number of the supplier.
    /// </summary>
    /// <value>The fax number.</value>
    /// <remarks>
    /// This property represents the fax number of the supplier. It is a string property
    /// with a maximum length of 24 characters.
    /// </remarks>
    [MaxLength(24)]
    public string? Fax { get; set; }

    /// <summary>
    /// Gets or sets the homepage URL for the supplier.
    /// </summary>
    public string? HomePage { get; set; }
}