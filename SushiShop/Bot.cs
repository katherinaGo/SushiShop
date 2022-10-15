using Newtonsoft.Json;
using Spectre.Console;

namespace SushiShop;

public class Bot
{
    private Customer _customer;
    private List<Sushi> _sushiMenu;
    private Order _order;
    private bool _isPaid;
    private ConsoleViewController _console;
    private string _tastySmile = ":face_savoring_food:";
    private string _sushiSmile = ":sushi:";

    public Bot()
    {
        _customer = new Customer();
        _order = new Order();
        _console = new ConsoleViewController();
    }

    public void SayHelloToCustomer()
    {
        AnsiConsole.Write(new Markup(
            $"[steelblue1]Hello, [bold]dear[/]! " +
            $"You are in small sushi-market. The best sushi in the world! {_sushiSmile} " +
            $"Please, choose, buy and enjoy {_tastySmile}![/]\n"));
        _customer.AmountOfMoney = 220;
    }

    public void AskCustomerWhatAddToCart()
    {
        int counter = 0;
        AnsiConsole.Write(new Markup("[yellow]Do you want to make an order of sushi?[/]\n"));
        do
        {
            if (counter > 0)
            {
                AnsiConsole.Write(
                    new Markup($"[yellow]Do you want to add more sushi {_sushiSmile} to your order?[/]\n"));
            }

            string customerInput =
                _console.DisplayMakeChoice("Choose [purple]Yes[/] or [orange1]No[/]", new[] { "Yes", "No" });
            if (customerInput.Equals("yes"))
            {
                ShowMenu();
                customerInput =
                    _console.DisplayMakeChoice("[steelblue1]Choose position from the menu below.[/]", _sushiMenu);

                for (int i = 0; i < _sushiMenu.Count; i++)
                {
                    if (_sushiMenu[i].ToString().Equals(customerInput))
                    {
                        _order.CartWithSushi.Add(_sushiMenu[i]);
                    }
                }

                AnsiConsole.Write(new Markup($"[steelblue1]'{customerInput}' was added to Cart.[/]\n"));
                counter++;
            }
            else if (customerInput.Equals("no"))
            {
                AnsiConsole.Write(new Markup("[steelblue1]Thank you![/]\n"));
                break;
            }
        } while (true);
    }

    public void ShowCustomerItemsInCart()
    {
        int totalPrice = 0;
        int counter = 1;
        foreach (var item in _order.CartWithSushi)
        {
            AnsiConsole.Write(new Markup($"[steelblue1]{counter}. {item} in your Cart.[/]\n"));

            totalPrice += item.Price;
            counter++;
        }

        if (totalPrice != 0)
        {
            AnsiConsole.Write(new Markup($"[steelblue1]Total price: {totalPrice}.[/]\n"));
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
                if (string.IsNullOrWhiteSpace(_customer.Name))
                {
                    AnsiConsole.Write(
                        new Markup("[red]Field can't be empty.[/]\n"));
                }
                else
                {
                    AnsiConsole.Write(new Markup($"[yellow]Was the name typed correctly?[/]\n"));
                    customerInput =
                        _console.DisplayMakeChoice("Choose [purple]Yes[/] for confirming or [orange1]No[/] to change.",
                            new[] { "Yes", "No" });

                    if (customerInput.Equals("yes"))
                    {
                        AnsiConsole.Write(new Markup("[steelblue1]OK, saved.[/]\n"));
                        break;
                    }
                }
            } while (true);

            do
            {
                _customer.Surname = AskCustomerSurname();
                if (string.IsNullOrWhiteSpace(_customer.Surname))
                {
                    AnsiConsole.Write(new Markup($"[red]Field can't be empty.[/]\n"));
                }
                else
                {
                    AnsiConsole.Write(new Markup("[yellow]Was the surname typed correctly?[/]\n"));

                    customerInput =
                        _console.DisplayMakeChoice(
                            "[steelblue1]Choose [purple]Yes[/] for confirming or [orange1]No[/] to change.[/]",
                            new[] { "Yes", "No" });

                    if (customerInput.Equals("yes"))
                    {
                        AnsiConsole.Write(new Markup("[steelblue1]OK, saved.[/]\n"));
                        break;
                    }
                }
            } while (true);

            do
            {
                _customer.Address = AskCustomerAddress();
                if (string.IsNullOrWhiteSpace(_customer.Address))
                {
                    AnsiConsole.Write(new Markup("[red]Field can't be empty.[/]\n"));
                }
                else
                {
                    AnsiConsole.Write(new Markup("[yellow]Was the address typed correctly?[/]"));
                    customerInput =
                        _console.DisplayMakeChoice(
                            "[steelblue1]Choose [purple]Yes[/] for confirming or [orange1]No[/] to change.[/]",
                            new[] { "Yes", "No" });

                    if (customerInput.Equals("yes"))
                    {
                        AnsiConsole.Write(new Markup("[steelblue1]OK, saved.[/]\n"));
                        break;
                    }
                }
            } while (true);

            AnsiConsole.Write(
                new Markup(
                    $"[steelblue1]Dear [bold]{_customer.Name} {_customer.Surname}[/], your order will be delivered to the following address: "
                    + $"[bold]{_customer.Address}[/] during 2h after successfully payment.[/]\n"));
        }
        else
        {
            AnsiConsole.Write(new Markup("[steelblue1]Good bye! Have a nice day![/]"));
        }
    }

    public void AskCustomerToPay()
    {
        if (_order.CartWithSushi.Count != 0)
        {
            AnsiConsole.Write(new Markup(
                $"[steelblue1]You have to pay [bold]{_order.TotalOrderPrice}€[/] for your order. " +
                $"Your current balance: [bold]{_customer.AmountOfMoney}€[/].[/]\n"));
            if (CheckIfCustomerHaveEnoughMoneyToPay().Equals(true))
            {
                AnsiConsole.Write(new Markup("[yellow]Do you confirm you order and ready to pay?[/]\n"));
                string customerInput = _console.DisplayMakeChoice(
                    "[steelblue1]Choose [purple]Yes[/] for confirming or [orange1]No[/] to change.[/]",
                    new[] { "Yes", "No" });

                if (customerInput.Equals("yes"))
                {
                    AnsiConsole.Write(new Markup("[steelblue1]Getting your money...[/]"));
                    _customer.AmountOfMoney -= _order.TotalOrderPrice;
                    AnsiConsole.Write(new Markup(
                        $"[steelblue1]Successfully paid. Your current balance:  [bold]{_customer.AmountOfMoney}€[/].[/]\n"));
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
            AnsiConsole.Write(new Markup(
                $"[steelblue1]Dear [bold]{_customer.Name} {_customer.Surname}[/], " +
                $"your order: is going to be delivered to [bold]{_customer.Address}[/] in 2 hours. \nOrder:\n [/]"));
            foreach (var item in _order.CartWithSushi)
            {
                AnsiConsole.Write(new Markup($"[steelblue1]{item}[/]\n"));
            }
        }
    }

    private void ShowMenu()
    {
        AnsiConsole.Write(new Markup("[steelblue1]Today in the menu:[/]\n"));
        _sushiMenu = ParseMenuFromJson();
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
        AnsiConsole.Write(new Markup("[steelblue1]Tell us your name, please...[/]\n"));
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
        AnsiConsole.Write(new Markup("[steelblue1]Tell us your surname, please...[/]\n"));

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
        AnsiConsole.Write(new Markup(
            "[steelblue1]Tell us your address for delivery in format [italic]'Street, # of house, # of flat'[/].[/]\n"));

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