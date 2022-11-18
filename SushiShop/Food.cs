using Newtonsoft.Json;

namespace SushiShop;

public class Food
{
    [JsonProperty("sushi")] public List<Sushi>? Sushi { get; set; }
}