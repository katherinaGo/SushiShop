namespace SushiShop;

public class Order
{
    public List<Sushi> CartWithSushi { get; set; }

    public Order()
    {
        CartWithSushi = new List<Sushi>();
    }
}