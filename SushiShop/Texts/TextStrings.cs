namespace SushiShop.Texts;

public static class TextStrings
{
    private const string SushiSmile = ":sushi:";
    private const string TastySmile = ":face_savoring_food:";

    private static Customer? _customer = new()
    {
        AmountOfMoney = 220
    };

    private static Dictionary<Keys, string>? _values;

    private static void SetTextToValues()
    {
        if (_values?.Count > 0)
        {
            _values.Clear();
        }

        _values = new Dictionary<Keys, string>
        {
            {
                Keys.SayHello, "[steelblue1]Hello, [bold]dear[/]! "
                               + $"You are in small sushi-market. The best sushi in the world! {SushiSmile} Choose, buy and enjoy {TastySmile}![/]\n"
            },
            {
                Keys.AskIfWantToMakeOrder, "[yellow]Do you want to make an order of sushi?[/]\n"
            },
            {
                Keys.AskIfAddSushiToOrder, $"[yellow]Do you want to add sushi {SushiSmile} to your order?[/]\n"
            },
            {
                Keys.ChoosePositionFromMenu, "[steelblue1]Choose position from the menu below.[/]"
            },
            {
                Keys.ThankYouMessage, "[steelblue1]OK, thank you![/]\n"
            },
            {
                Keys.TotalPriceInfo, "[steelblue1]Total order price: "
            },
            {
                Keys.DeliveredInfo, $"[steelblue1]Dear [bold]{_customer!.Name} {_customer.Surname}[/], "
                                    + "your order will be delivered to the following address: "
                                    + $"[bold]{_customer.Address}[/] during 1.5h after successfully payment.[/]\n"
            },
            {
                Keys.ByeMessage, "[steelblue1]Good bye! Have a nice day![/]"
            },
            {
                Keys.HowMuchToPayMessage, "[steelblue1]You have to pay for your order: [bold][/]"
            },
            {
                Keys.CustomerBalanceInfo, "\n[steelblue1]Your current balance: "
                                          + $"[bold]{_customer.AmountOfMoney}€[/].[/]\n"
            },
            {
                Keys.AskToConfirmPayment, "[yellow]Do you confirm you order and ready to pay €€€?[/]\n"
            },
            {
                Keys.YesOrNoChoice, "[steelblue1]Choose [purple]Yes[/] for confirming or [orange1]No[/] to change.[/]"
            },
            {
                Keys.LoaderToGetMoney, "[steelblue1]Getting your money...[/]"
            },
            {
                Keys.CurrentBalanceInfo, "[steelblue1]Successfully paid. "
                                         + $"\nYour current balance: [bold]{_customer.AmountOfMoney}€[/].[/]\n"
            },
            {
                Keys.FinalDeliveredInfo, $"[steelblue1]Dear [bold]{_customer.Name} {_customer.Surname}[/], "
                                         + $"your order is going to be delivered to [bold]{_customer.Address}[/]. \nOrder:[/]\n"
            },
            {
                Keys.TodayInMenu, "[steelblue1]Today in the menu:[/]\n"
            },
            {
                Keys.HowManyItemsToAdd, "[yellow]How many items of this position do you want to add? Type number.[/]\n"
            },
            {
                Keys.DontHaveThisAmount, "[steelblue1]We don't have so many item. "
            },
            {
                Keys.NotNumberError, "It's not a number. Try again.\n"
            },
            {
                Keys.TellName, "[steelblue1]Tell us your name, please...[/]\n"
            },
            {
                Keys.TellSurname, "[steelblue1]Tell us your surname, please...[/]\n"
            },
            {
                Keys.TellAddress, "[steelblue1]Tell us your address for delivery in format "
                                  + "[italic]'Street, # of house, # of flat'[/].[/]\n"
            },
            {
                Keys.TellEmail, "[steelblue1]Tell us your email, please...[/]\n"
            },
            {
                Keys.EmptyFieldError, "[red]Field can't be empty.[/]\n"
            },
            {
                Keys.InvalidEmailFormat, "[red]Invalid email format.[/]\n"
            },
            {
                Keys.WasTypedCorrectly, "[yellow]Was it typed correctly?[/]\n"
            },
            {
                Keys.SavedMessage, "[steelblue1]OK, saved.[/]\n"
            },
            {
                Keys.EmailSentToCustomer,
                $"[steelblue1]Email with order details was sent to [bold]{_customer.Email}[/].[/]"
            },
            {
                Keys.DontHaveEnoughMoney, "[red]You don't have enough money to pay for the order.[/]\n"
            }
        };
    }

    public static string GetString(Keys keys)
    {
        SetTextToValues();
        return _values![keys];
    }

    public static string GetString(Keys keys, Customer? customer)
    {
        _customer = customer;
        SetTextToValues();
        return _values![keys];
    }
}