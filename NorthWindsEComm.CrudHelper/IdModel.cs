using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NorthWindsEComm.CrudHelper;

public interface IIdModel
{
    [IgnoreDataMember]
    [JsonIgnore]
    public int Id { get; }
}