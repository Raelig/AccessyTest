

using System.Text.Json.Serialization;

namespace Acessy.Model
{
    public class Operation
    {
        [JsonPropertyName("id")]
       // [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        //[JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        //[JsonProperty(PropertyName = "lockId")]
        [JsonIgnore]
        public Guid LockId { get; set; }


    }
}
