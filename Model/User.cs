using System.Text.Json.Serialization;

namespace Acessy.Model
{
    public class User
    {
        [JsonPropertyName("username")] //rot registering so deviating from camelcase naming for now
        public string username { get; set; }
        [JsonPropertyName("password")]
        public string password { get; set; }
    }
}
