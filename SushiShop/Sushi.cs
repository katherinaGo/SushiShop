using System.Text.Json.Serialization;

namespace SushiShop;

public class Sushi : Food
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("amount")] public int Amount { get; set; }

    [JsonPropertyName("price")] public int Price { get; set; }


    public override string ToString()
    {
        return $"Sushi {Name}, {Amount} items for {Price}€";
    }
}