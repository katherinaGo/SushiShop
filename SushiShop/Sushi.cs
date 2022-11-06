using Newtonsoft.Json;

namespace SushiShop;

public class Sushi : Food
{
    [JsonProperty("name")] public string? Name { get; set; }

    [JsonProperty("amountInPortion")] public int AmountInPortion { get; set; }

    [JsonProperty("price")] public int Price { get; set; }

    [JsonProperty("availableForSell")] public int AvailableAmountForSell { get; set; }

    [JsonIgnore] public int NumberItemWasOrdered { get; set; }

    public override string ToString()
    {
        return $"Sushi {Name}, {AmountInPortion} items for {Price}€";
    }
}