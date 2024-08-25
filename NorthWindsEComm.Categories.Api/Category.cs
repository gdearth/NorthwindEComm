using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using NorthWindsEComm.CrudHelper;

public class Category : IIdModel
{
    [IgnoreDataMember]
    [JsonIgnore]
    public int Id => CategoryId; 
    [Key] public int CategoryId { get; set; }
    [MaxLength(15)] public string CategoryName { get; set; }
    public string Description { get; set; }
}