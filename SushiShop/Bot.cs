using System.Text.RegularExpressions;
using SushiShop.ConsoleController;
using SushiShop.EmailService;
using SushiShop.Texts;

namespace SushiShop;

public class Bot
{
    private readonly Customer? _customer;
    private readonly List<Sushi> _sushiMenu;
    private readonly Order _order;
    private bool _isPaid;
    private int _amountToOrder;
    private event Action TakenCustomerMoney;
    private event Action? ShowItemsInCart;
    private event Func<Customer, bool> EmailSentToCustomerAfterPaying;

    public Bot()
    {
        _customer = new Customer
        {
            AmountOfMoney = 220
        };
        _order = new Order();
        TakenCustomerMoney = PayForOrder;
        _sushiMenu = MenuController.ParseMenuFromJson();
        EmailSentToCustomerAfterPaying = EmailSender.IsSentEmailWithResults;
    }

    public void StartProgram()
    {
        SayHelloToCustomer();
        if (AskCustomerIfAddItemsToCart())
        {
            AskCustomerBillingInfo();
            AskCustomerToPay();
            if (_isPaid)
            {
                ShowCustomerItemsToDeliver();
            }
            else
            {
                TakenCustomerMoney -= PayForOrder;
            }
        }
    }

    private bool IsPossibleAddPositionsToCart(string? itemToOrder)
    {
        do
        {
            MenuController.PrintToConsole(TextStrings.GetString(Keys.HowManyItemsToAdd));
            if (int.TryParse(Console.ReadLine()!, out _amountToOrder).Equals(true))
            {
                foreach (var item in _sushiMenu)
                {
                    if (item.ToString().Equals(itemToOrder) && item.AvailableAmountForSell < _amountToOrder)
                    {
                        MenuController.PrintToConsole(TextStrings.GetString(Keys.DontHaveThisAmount)
                                                      + $"Available: {item.AvailableAmountForSell} items(s) of {item.Name}.[/]\n");
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
                MenuController.PrintToConsole(TextStrings.GetString(Keys.NotNumberError));
            }
        } while (true);
    }

    private void AskCustomerName()
    {
        do
        {
            MenuController.PrintToConsole(TextStrings.GetString(Keys.TellName));

            var customerInput = Console.ReadLine()!;
            _customer!.Name = customerInput;

            if (string.IsNullOrWhiteSpace(_customer.Name))
            {
                MenuController.PrintToConsole(TextStrings.GetString(Keys.EmptyFieldError));
            }
            else
            {
                MenuController.PrintToConsole(TextStrings.GetString(Keys.WasTypedCorrectly));
                customerInput =
                    ConsoleViewController.DisplayMakeChoice(
                        TextStrings.GetString(Keys.YesOrNoChoice),
                        new[] { "Yes", "No" });

                if (customerInput!.Equals("yes"))
                {
                    MenuController.PrintToConsole(TextStrings.GetString(Keys.SavedMessage));
                    break;
                }
            }
        } while (true);
    }

    private void AskCustomerSurname()
    {
        do
        {
            MenuController.PrintToConsole(TextStrings.GetString(Keys.TellSurname));

            var customerInput = Console.ReadLine()!;
            _customer!.Surname = customerInput;

            if (string.IsNullOrWhiteSpace(_customer.Surname))
            {
                MenuController.PrintToConsole(TextStrings.GetString(Keys.EmptyFieldError));
            }
            else
            {
                MenuController.PrintToConsole(TextStrings.GetString(Keys.WasTypedCorrectly));

                customerInput =
                    ConsoleViewController.DisplayMakeChoice(
                        TextStrings.GetString(Keys.YesOrNoChoice),
                        new[] { "Yes", "No" });

                if (customerInput!.Equals("yes"))
                {
                    MenuController.PrintToConsole(TextStrings.GetString(Keys.SavedMessage));
                    break;
                }
            }
        } while (true);
    }

    private void AskCustomerAddress()
    {
        do
        {
            MenuController.PrintToConsole(TextStrings.GetString(Keys.TellAddress));

            var customerInput = Console.ReadLine()!;
            _customer!.Address = customerInput;

            if (string.IsNullOrWhiteSpace(_customer.Address))
            {
                MenuController.PrintToConsole(TextStrings.GetString(Keys.EmptyFieldError));
            }
            else
            {
                MenuController.PrintToConsole(TextStrings.GetString(Keys.WasTypedCorrectly));
                customerInput =
                    ConsoleViewController.DisplayMakeChoice(
                        TextStrings.GetString(Keys.YesOrNoChoice),
                        new[] { "Yes", "No" });

                if (customerInput!.Equals("yes"))
                {
                    MenuController.PrintToConsole(TextStrings.GetString(Keys.SavedMessage));
                    break;
                }
            }
        } while (true);
    }

    private void AskCustomerEmail()
    {
        do
        {
            MenuController.PrintToConsole(TextStrings.GetString(Keys.TellEmail));

            var customerInput = Console.ReadLine()!;
            _customer!.Email = customerInput;
            var isEmail = Regex.IsMatch(_customer.Email,
                @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?",
                RegexOptions.IgnoreCase);
            if (string.IsNullOrWhiteSpace(_customer.Email) || isEmail.Equals(false))
            {
                MenuController.PrintToConsole(TextStrings.GetString(Keys.InvalidEmailFormat));
            }
            else
            {
                MenuController.PrintToConsole(TextStrings.GetString(Keys.WasTypedCorrectly));
                customerInput =
                    ConsoleViewController.DisplayMakeChoice(
                        TextStrings.GetString(Keys.YesOrNoChoice),
                        new[] { "Yes", "No" });

                if (customerInput!.Equals("yes"))
                {
                    MenuController.PrintToConsole(TextStrings.GetString(Keys.SavedMessage));
                    break;
                }
            }
        } while (true);
    }

    private bool CheckIfCustomerHaveEnoughMoneyToPay()
    {
        return _customer!.AmountOfMoney >= _order.TotalOrderPrice;
    }

    private void PayForOrder()
    {
        MenuController.PrintToConsole((TextStrings.GetString(Keys.LoaderToGetMoney)));
        _customer!.AmountOfMoney -= _order.TotalOrderPrice;
        Thread.Sleep(3500);
        MenuController.PrintToConsole(TextStrings.GetString(Keys.CurrentBalanceInfo, _customer));
        _isPaid = true;
    }

    private static void SayHelloToCustomer()
    {
        MenuController.PrintToConsole(TextStrings.GetString(Keys.SayHello));
    }

    private bool AskCustomerIfAddItemsToCart()
    {
        MenuController.ShowMenu(_sushiMenu);
        do
        {
            MenuController.PrintToConsole(TextStrings.GetString(Keys.AskIfAddSushiToOrder));

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
                        ShowItemsInCart = ShowCustomerCart;
                    }

                    MenuController.PrintToConsole(
                        $"[steelblue1]{_amountToOrder} of '{customerInput}' added to Cart.[/]\n");
                }
            }
            else if (customerInput.Equals("no"))
            {
                MenuController.PrintToConsole(TextStrings.GetString(Keys.ThankYouMessage));
                break;
            }
        } while (true);

        if (_order.CartWithSushi.Count != 0)
        {
            ShowItemsInCart!();
            return true;
        }

        ShowItemsInCart -= ShowCustomerCart;
        return false;
    }

    private void ShowCustomerCart()
    {
        var totalPrice = 0;
        var counter = 1;
        foreach (var item in _order.CartWithSushi)
        {
            MenuController.PrintToConsole(
                $"[steelblue1]{counter}. {item}, {item.NumberItemWasOrdered} position(s) in your Cart.[/]\n");

            totalPrice += item.Price * item.NumberItemWasOrdered;
            counter++;
        }

        if (totalPrice != 0)
        {
            MenuController.PrintToConsole(TextStrings.GetString(Keys.TotalPriceInfo) + $" {totalPrice}€[/]\n");
        }

        _order.TotalOrderPrice = totalPrice;
    }

    private void AskCustomerBillingInfo()
    {
        AskCustomerName();
        AskCustomerSurname();
        AskCustomerAddress();
        AskCustomerEmail();

        MenuController.PrintToConsole(TextStrings.GetString(Keys.DeliveredInfo, _customer));
    }

    private void AskCustomerToPay()
    {
        MenuController.PrintToConsole(TextStrings.GetString(Keys.HowMuchToPayMessage)
                                      + $"{_order.TotalOrderPrice}€[/]");
        MenuController.PrintToConsole(TextStrings.GetString(Keys.CustomerBalanceInfo));
        if (CheckIfCustomerHaveEnoughMoneyToPay())
        {
            MenuController.PrintToConsole(TextStrings.GetString(Keys.AskToConfirmPayment));
            var customerInput = ConsoleViewController.DisplayMakeChoice(
                TextStrings.GetString(Keys.YesOrNoChoice),
                new[] { "Yes", "No" });

            if (customerInput!.Equals("yes"))
            {
                TakenCustomerMoney();
            }
        }
    }

    private void ShowCustomerItemsToDeliver()
    {
        MenuController.PrintToConsole(
            TextStrings.GetString(Keys.FinalDeliveredInfo, _customer));
        foreach (var item in _order.CartWithSushi)
        {
            MenuController.PrintToConsole($"[steelblue1]'{item}' - {item.NumberItemWasOrdered} position(s)[/]\n");
        }

        if (EmailSentToCustomerAfterPaying(_customer!))
        {
            MenuController.PrintToConsole(TextStrings.GetString(Keys.EmailSentToCustomer));
        }

        // TODO update menu.json file to rewrite 'availableForSell' value
    }
}