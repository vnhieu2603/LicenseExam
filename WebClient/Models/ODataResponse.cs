using Newtonsoft.Json;

namespace WebClient.Models
{
    public class ODataResponse<T>
    {
        [JsonProperty("@odata.context")]
        public string Context { get; set; }

        [JsonProperty("value")]
        public List<T> Value { get; set; }
    }
}
