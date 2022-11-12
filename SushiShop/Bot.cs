using System.Text.RegularExpressions;
using Spectre.Console;
using SushiShop.ConsoleController;
using SushiShop.EmailService;
using SushiShop.Texts;

namespace SushiShop;

public delegate void AccountHandler(bool isPaid);

public delegate void CartHandler();

public class Bot
{
    private readonly Customer? _customer;
    private readonly List<Sushi> _sushiMenu;
    private readonly Order _order;
    private bool _isPaid;
    private int _amountToOrder;
    private event AccountHandler TakeCustomerMoney;
    private event CartHandler ItemsInCart;
    private event SendEmailIfPaid SendEmailToCustomer;

    public Bot()
    {
        _customer = new Customer
        {
            AmountOfMoney = 220
        };
        _order = new Order();
        _sushiMenu = MenuController.ParseMenuFromJson();
        TakeCustomerMoney = PayForOrder;
        ItemsInCart = ShowCustomerCart;
        SendEmailToCustomer = EmailSender.SendEmailWithResults;
    }

    public void StartProgram()
    {
        SayHelloToCustomer();
        var isItemsAdded = AskCustomerIfAddItemsToCart();
        if (isItemsAdded)
        {
            AskCustomerBillingInfo();
            AskCustomerToPay();
            if (_isPaid)
            {
                ShowCustomerItemsToDeliver();
            }
        }
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

    private void AskCustomerEmail()
    {
        do
        {
            AnsiConsole.Write(new Markup(
                TextStrings.GetString(Keys.TellEmail)));

            var customerInput = Console.ReadLine()!;
            _customer!.Email = customerInput;
            var isEmail = Regex.IsMatch(_customer.Email,
                @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?",
                RegexOptions.IgnoreCase);
            if (string.IsNullOrWhiteSpace(_customer.Email) || isEmail.Equals(false))
            {
                AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.InvalidEmailFormat)));
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

    private void PayForOrder(bool isPaid)
    {
        AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.LoaderToGetMoney)));
        _customer!.AmountOfMoney -= _order.TotalOrderPrice;
        Thread.Sleep(3500);
        AnsiConsole.Write(new Markup(
            TextStrings.GetString(Keys.CurrentBalanceInfo, _customer)));
        _isPaid = isPaid;
    }

    private static void SayHelloToCustomer()
    {
        AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.SayHello)));
    }

    private bool AskCustomerIfAddItemsToCart()
    {
        MenuController.ShowMenu(_sushiMenu);
        do
        {
            AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.AskIfAddSushiToOrder)));

            var customerInput = ConsoleViewController.DisplayMakeChoice(
                TextStrings.GetString(Keys.YesOrNoChoice),
                new[] { "Yes", "No" }
            );
            if (customerInput!.Equals("yes"))
            {
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
                break;
            }
        } while (true);

        if (_order.CartWithSushi.Count != 0)
        {
            ItemsInCart();
            return true;
        }

        return false;
    }

    private void ShowCustomerCart()
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
            AnsiConsole.Write(new Markup(TextStrings.GetString(Keys.TotalPriceInfo) + $" {totalPrice}€[/]\n"));
        }

        _order.TotalOrderPrice = totalPrice;
    }

    private void AskCustomerBillingInfo()
    {
        AskCustomerName();
        AskCustomerSurname();
        AskCustomerAddress();
        AskCustomerEmail();

        AnsiConsole.Write(
            new Markup(TextStrings.GetString(Keys.DeliveredInfo, _customer)));
    }

    private void AskCustomerToPay()
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
                TakeCustomerMoney(true);
            }
        }
    }

    private void ShowCustomerItemsToDeliver()
    {
        AnsiConsole.Write(new Markup(
            TextStrings.GetString(Keys.FinalDeliveredInfo, _customer)));
        foreach (var item in _order.CartWithSushi)
        {
            AnsiConsole.Write(new Markup($"[steelblue1]'{item}' - {item.NumberItemWasOrdered} position(s)[/]\n"));
        }

        SendEmailToCustomer(_customer!);
        // TODO update menu.json file to rewrite 'availableForSell' value
    }
}