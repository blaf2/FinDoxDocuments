using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinDoxDocumentsAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public UserTypes UserType { get; set; }
    }
}
