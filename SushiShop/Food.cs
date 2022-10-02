using System.Text.Json.Serialization;

namespace SushiShop;

public class Food
{
    [JsonPropertyName("sushi")] public List<Sushi> Sushi { get; set; }
}