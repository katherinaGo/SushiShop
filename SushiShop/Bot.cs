using System.Globalization;
using Newtonsoft.Json;

namespace SushiShop;

public class Bot
{
    private Customer _customer;
    private List<Sushi> _sushiMenu;
    private Order _order;
    private bool _isPaid;
    private ConsoleViewController _console;

    public Bot()
    {
        _customer = new Customer();
        _order = new Order();
    }

    public void SayHelloToCustomer()
    {
        Console.WriteLine(
            "Hello, dear! You are in small sushi-market. The best sushi in the world! Please, choose, buy and enjoy!");
        _customer.AmountOfMoney = 220;
    }

    public void AskCustomerWhatAddToCart()
    {
        int counter = 0;
        Console.WriteLine("Do you want to make an order of sushi add it to cart? Type Y or N.");
        do
        {
            if (counter > 0)
            {
                Console.WriteLine("Do you want to add more sushi to your order? Type Y or N.");
            }

            string customerInput = Console.ReadLine()?.ToLower()!;
            if (customerInput.Equals("y"))
            {
                ShowMenu();
                Console.WriteLine("What position do you want to order? Type number from the menu above. ");
                customerInput = Console.ReadLine()!;
                try
                {
                    int position = int.Parse(customerInput, NumberStyles.Integer);

                    for (int i = 0; i < _sushiMenu.Count; i++)
                    {
                        _order.CartWithSushi.Add(_sushiMenu[position - 1]);
                        Console.WriteLine($"Position #{position} '{_sushiMenu[position - 1]}' was added to Cart.");
                        break;
                    }

                    counter++;
                }
                catch (Exception e)
                {
                    Console.WriteLine("We don't have such position in the menu.");
                    Console.WriteLine("Do you want to add more sushi to your order? Type Y or N.");
                }
            }
            else if (customerInput.Equals("n"))
            {
                Console.WriteLine("Thank you!");
                break;
            }
            else
            {
                Console.WriteLine("No such option.");
                Console.WriteLine("Do you want to add more sushi to your order? Type Y or N.");
            }
        } while (true);
    }

    public void ShowCustomerItemsInCart()
    {
        int totalPrice = 0;
        int counter = 1;
        foreach (var item in _order.CartWithSushi)
        {
            Console.WriteLine($"{counter}. {item} in your Cart.");
            totalPrice += item.Price;
            counter++;
        }

        if (totalPrice == 0)
        {
            Console.WriteLine("Your cart is empty! You have nothing to pay for.");
        }

        else
        {
            Console.WriteLine($"Total price: {totalPrice}");
        }

        _order.TotalOrderPrice = totalPrice;
    }

    public void AskCustomerForNameAndAddressForOrder()
    {
        if (_order.CartWithSushi.Count != 0)
        {
            string customerInput;
            do
            {
                _customer.Name = AskCustomerName();
                Console.WriteLine("Was the name typed correctly?  Type Y for confirming or N to change.");
                customerInput = Console.ReadLine()?.ToLower()!;
                if (customerInput.Equals("y"))
                {
                    break;
                }

                if (!customerInput.Equals("n"))
                {
                    Console.WriteLine("No such option.");
                }
            } while (true);

            do
            {
                _customer.Surname = AskCustomerSurname();
                Console.WriteLine("Was the surname typed correctly?  Type Y for confirming or N to change.");
                customerInput = Console.ReadLine()?.ToLower()!;
                if (customerInput.Equals("y"))
                {
                    break;
                }

                if (!customerInput.Equals("n"))
                {
                    Console.WriteLine("No such option.");
                }
            } while (true);

            do
            {
                _customer.Address = AskCustomerAddress();
                Console.WriteLine("Was the address typed correctly?  Type Y for confirming or N to change.");
                customerInput = Console.ReadLine()?.ToLower()!;
                if (customerInput.Equals("y"))
                {
                    break;
                }

                if (!customerInput.Equals("n"))
                {
                    Console.WriteLine("No such option.");
                }
            } while (true);

            Console.WriteLine(
                $"Dear {_customer.Name} {_customer.Surname}, your order will be delivered to the following address: " +
                $"{_customer.Address} during 2h after successfully payment.");
        }
        else
        {
            Console.WriteLine("Good bye! Have a nice day!");
        }
    }

    public void AskCustomerToPay()
    {
        if (_order.CartWithSushi.Count != 0)
        {
            Console.WriteLine(
                $"You have to pay {_order.TotalOrderPrice}€ for your order. Your balance: {_customer.AmountOfMoney}€.");
            if (CheckIfCustomerHaveEnoughMoneyToPay().Equals(true))
            {
                Console.WriteLine("You have enough money on your balance to pay.");
                Console.WriteLine("Do you confirm you order and ready to pay? Type Y for confirming or N to cancel.");
                string customerInput = Console.ReadLine()?.ToLower()!;
                if (customerInput.Equals("y"))
                {
                    Console.WriteLine("Getting your money...");
                    _customer.AmountOfMoney -= _order.TotalOrderPrice;
                    Console.WriteLine($"Successfully paid. Your current balance:  {_customer.AmountOfMoney}€.");
                    _isPaid = true;
                }
            }
            else
            {
                _isPaid = false;
            }
        }
    }

    public void ShowCustomerItemsToDeliver()
    {
        if (_isPaid.Equals(true))
        {
            Console.Write(
                $"Dear {_customer.Name} {_customer.Surname}, your order: is going to be delivered to {_customer.Address} in 2 hours. \nOrder: ");
            foreach (var item in _order.CartWithSushi)
            {
                Console.WriteLine(item);
            }
        }
    }

    private void ShowMenu()
    {
        Console.WriteLine("Today in the menu:");
        _sushiMenu = ParseMenuFromJson();
        _console = new ConsoleViewController();
        for (int j = 0; j < _sushiMenu.Count; j++)
        {
            _console.AddItemToTable(_sushiMenu[j].ToString(), j + 1);
        }

        _console.PrintTableToConsole();
    }

    private List<Sushi> ParseMenuFromJson()
    {
        string jsonMenu = File.ReadAllText("/Users/kate/RiderProjects/SushiShop/SushiShop/menu.json");
        Food? collection = JsonConvert.DeserializeObject<Food>(jsonMenu);
        List<Sushi> sushiDish = collection?.Sushi!;
        return sushiDish;
    }

    private string AskCustomerName()
    {
        Console.WriteLine("Tell us your name, please...");
        try
        {
            _customer.Name = Console.ReadLine()!;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }

        return _customer.Name!;
    }

    private string AskCustomerSurname()
    {
        Console.WriteLine("Tell us your surname, please...");
        try
        {
            _customer.Surname = Console.ReadLine()!;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }

        return _customer.Surname!;
    }

    private string AskCustomerAddress()
    {
        Console.WriteLine("Tell us your address for delivery in format 'Street, # of house, # of flat', please...");
        try
        {
            _customer.Address = Console.ReadLine()!;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }

        return _customer.Address!;
    }

    private bool CheckIfCustomerHaveEnoughMoneyToPay()
    {
        if (_customer.AmountOfMoney >= _order.TotalOrderPrice)
        {
            return true;
        }

        return false;
    }
}