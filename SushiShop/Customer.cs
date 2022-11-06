namespace SushiShop;

public class Customer : Person
{
    public int AmountOfMoney { get; set; }

    public Order? Order { get; set; }
}