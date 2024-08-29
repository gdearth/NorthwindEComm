using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using NorthWindsEComm.CrudHelper;

namespace NorthWindsEComm.Categories.Api;

/// <summary>
/// Represents a category entity.
/// </summary>
public class Category : IIdModel
{
    /// <summary>
    /// Represents the identifier of a Category.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public int Id => CategoryId;

    /// <summary>
    /// Represents the identifier of a category.
    /// </summary>
    [Key]
    public int CategoryId { get; set; }

    /// <summary>
    /// Represents the name of a category.
    /// </summary>
    [MaxLength(15)]
    public string CategoryName { get; set; }

    /// <summary>
    /// Represents the description of a category.
    /// </summary>
    public string Description { get; set; }
}