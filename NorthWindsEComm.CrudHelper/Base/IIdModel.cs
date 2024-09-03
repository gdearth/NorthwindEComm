using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NorthWindsEComm.CrudHelper.Base;

public interface IIdModel
{
    [IgnoreDataMember]
    [JsonIgnore]
    public int Id { get; }
}