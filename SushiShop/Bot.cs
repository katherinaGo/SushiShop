using Newtonsoft.Json;
using Spectre.Console;

namespace SushiShop;

public delegate void AccountHandler();

public delegate void CartHandler();

public class Bot
{
    private readonly Customer? _customer;
    private readonly List<Sushi> _sushiMenu;
    private readonly Order _order;
    private bool _isPaid;
    private int _amountToOrder;
    private readonly ConsoleViewController _console;
    private const string MenuPathFile = "/Users/kate/RiderProjects/SushiShop/SushiShop/menu.json";
    private event AccountHandler TakeCustomerMoney;
    private event CartHandler ItemsInCart;

    public Bot()
    {
        _customer = new Customer
        {
            AmountOfMoney = 220
        };
        _order = new Order();
        _console = new ConsoleViewController();
        _sushiMenu = ParseMenuFromJson();
        TakeCustomerMoney = PayForOrder;
        ItemsInCart = ShowCustomerCart;
    }

    private static List<Sushi> ParseMenuFromJson()
    {
        var jsonMenu = File.ReadAllText(MenuPathFile);
        var collection = JsonConvert.DeserializeObject<Food>(jsonMenu);
        var sushiDish = collection?.Sushi!;
        for (var i = 0; i < sushiDish.Count; i++)
        {
            if (sushiDish[i].AvailableAmountForSell == 0)
            {
                sushiDish.Remove(sushiDish[i]);
            }
        }

        return sushiDish;
    }

    private void ShowMenu()
    {
        AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.TodayInMenu)));
        for (var j = 0; j < _sushiMenu.Count; j++)
        {
            ConsoleViewController.AddItemToTable(_sushiMenu[j].ToString(), j + 1);
        }

        ConsoleViewController.PrintTableToConsole();
    }

    private bool IsPossibleAddPositionsToCart(string? itemToOrder)
    {
        do
        {
            AnsiConsole.Write(
                new Markup(TextStrings.GetString(Keys.HowManyItemsToAdd)));
            var customerInput = Console.ReadLine()!;
            if (int.TryParse(customerInput, out _amountToOrder).Equals(true))
            {
                foreach (var item in _sushiMenu)
                {
                    if (item.ToString().Equals(itemToOrder) && item.AvailableAmountForSell < _amountToOrder)
                    {
                        AnsiConsole.Write(new Markup(
                            TextStrings.GetString(Keys.DontHaveThisAmount) +
                            $"Available: {item.AvailableAmountForSell} items(s) of {item.Name}.[/]\n"));
                        return false;
                    }

                    if (item.ToString().Equals(itemToOrder) && item.AvailableAmountForSell >= _amountToOrder)
                    {
                        return true;
                    }
                }
            }
            else
            {
                AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.NotNumberError)));
            }
        } while (true);
    }

    private void AskCustomerName()
    {
        do
        {
            AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.TellName)));

            var customerInput = Console.ReadLine()!;
            _customer!.Name = customerInput;

            if (string.IsNullOrWhiteSpace(_customer.Name))
            {
                AnsiConsole.Write(
                    new Markup(TextStrings.GetString(Keys.EmptyFieldError)));
            }
            else
            {
                AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.WasTypedCorrectly)));
                customerInput =
                    ConsoleViewController.DisplayMakeChoice(
                        TextStrings.GetString(Keys.YesOrNoChoice),
                        new[] { "Yes", "No" });

                if (customerInput!.Equals("yes"))
                {
                    AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.SavedMessage)));
                    break;
                }
            }
        } while (true);
    }

    private void AskCustomerSurname()
    {
        do
        {
            AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.TellSurname)));

            var customerInput = Console.ReadLine()!;
            _customer!.Surname = customerInput;

            if (string.IsNullOrWhiteSpace(_customer.Surname))
            {
                AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.EmptyFieldError)));
            }
            else
            {
                AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.WasTypedCorrectly)));

                customerInput =
                    ConsoleViewController.DisplayMakeChoice(
                        TextStrings.GetString(Keys.YesOrNoChoice),
                        new[] { "Yes", "No" });

                if (customerInput!.Equals("yes"))
                {
                    AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.SavedMessage)));
                    break;
                }
            }
        } while (true);
    }

    private void AskCustomerAddress()
    {
        do
        {
            AnsiConsole.Write(new Markup(
                TextStrings.GetString(Keys.TellAddress)));

            var customerInput = Console.ReadLine()!;
            _customer!.Address = customerInput;

            if (string.IsNullOrWhiteSpace(_customer.Address))
            {
                AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.EmptyFieldError)));
            }
            else
            {
                AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.WasTypedCorrectly)));
                customerInput =
                    ConsoleViewController.DisplayMakeChoice(
                        TextStrings.GetString(Keys.YesOrNoChoice),
                        new[] { "Yes", "No" });

                if (customerInput!.Equals("yes"))
                {
                    AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.SavedMessage)));
                    break;
                }
            }
        } while (true);
    }

    private bool CheckIfCustomerHaveEnoughMoneyToPay()
    {
        return _customer!.AmountOfMoney >= _order.TotalOrderPrice;
    }

    private void UpdateMenu(List<Sushi> sushi)
    {
        var json = JsonConvert.SerializeObject(sushi);
        File.WriteAllText(MenuPathFile, json);
    }

    private void PayForOrder()
    {
        AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.LoaderToGetMoney)));
        _customer!.AmountOfMoney -= _order.TotalOrderPrice;
        Thread.Sleep(3500);
        AnsiConsole.Write(new Markup(
            TextStrings.GetString(Keys.CurrentBalanceInfo, _customer)));
        _isPaid = true;
    }
    
    public void SayHelloToCustomer()
    {
        AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.SayHello)));
    }

    public void AskCustomerIfAddItemsToCart()
    {
        do
        {
            AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.AskIfAddSushiToOrder)));

            var customerInput = ConsoleViewController.DisplayMakeChoice(
                TextStrings.GetString(Keys.YesOrNoChoice),
                new[] { "Yes", "No" }
            );
            if (customerInput!.Equals("yes"))
            {
                ShowMenu();
                customerInput =
                    ConsoleViewController.DisplayMakeChoice(
                        TextStrings.GetString(Keys.ChoosePositionFromMenu),
                        _sushiMenu);

                if (IsPossibleAddPositionsToCart(customerInput))
                {
                    foreach (var item in _sushiMenu.Where(item => item.ToString().Equals(customerInput)))
                    {
                        _order.CartWithSushi.Add(item);
                        item.AvailableAmountForSell -= _amountToOrder;
                        item.NumberItemWasOrdered = _amountToOrder;
                    }

                    AnsiConsole.Write(
                        new Markup($"[steelblue1]{_amountToOrder} of '{customerInput}' added to Cart.[/]\n"));
                }
            }
            else if (customerInput.Equals("no"))
            {
                AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.ThankYouMessage)));
                ItemsInCart();
                break;
            }
        } while (true);
    }

    public void ShowCustomerCart()
    {
        var totalPrice = 0;
        var counter = 1;
        foreach (var item in _order.CartWithSushi)
        {
            AnsiConsole.Write(new Markup(
                $"[steelblue1]{counter}. {item}, {item.NumberItemWasOrdered} position(s) in your Cart.[/]\n"));

            totalPrice += item.Price * item.NumberItemWasOrdered;
            counter++;
        }

        if (totalPrice != 0)
        {
            AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.TotalPriceInfo) + $" {totalPrice} [/]\n"));
        }

        _order.TotalOrderPrice = totalPrice;
    }

    public void AskCustomerBillingInfo()
    {
        if (_order.CartWithSushi.Count != 0)
        {
            AskCustomerName();
            AskCustomerSurname();
            AskCustomerAddress();

            AnsiConsole.Write(
                new Markup(TextStrings.GetString(Keys.DeliveredInfo, _customer)));
        }
        else
        {
            AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.ByeMessage)));
        }
    }

    public void AskCustomerToPay()
    {
        if (_order.CartWithSushi.Count != 0)
        {
            AnsiConsole.Write(new Markup(
                TextStrings.GetString(Keys.HowMuchToPayMessage) + $"{_order.TotalOrderPrice}€[/]"));
            AnsiConsole.Write(new Markup(
                TextStrings.GetString(Keys.CustomerBalanceInfo)));
            if (CheckIfCustomerHaveEnoughMoneyToPay())
            {
                AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.AskToConfirmPayment)));
                var customerInput = ConsoleViewController.DisplayMakeChoice(
                    TextStrings.GetString(Keys.YesOrNoChoice),
                    new[] { "Yes", "No" });

                if (customerInput!.Equals("yes"))
                {
                    TakeCustomerMoney();
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
        if (_isPaid)
        {
            AnsiConsole.Write(new Markup(
                TextStrings.GetString(Keys.FinalDeliveredInfo, _customer)));
            foreach (var item in _order.CartWithSushi)
            {
                AnsiConsole.Write(new Markup($"[steelblue1]'{item}' - {item.NumberItemWasOrdered} position(s)[/]\n"));
            }
            // TODO update menu.json file to rewrite 'availableForSell' value
        }
    }
}