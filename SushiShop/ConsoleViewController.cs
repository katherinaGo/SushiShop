using Spectre.Console;

namespace SushiShop;

public class ConsoleViewController
{
    private static Table? _table;
    private string _sushiSmile = ":sushi:";

    public ConsoleViewController()
    {
        _table = new Table();
        _table.Title("[blue]MENU[/]");
        _table.AddColumn("#");
        _table.AddColumn(
            new TableColumn($"{_sushiSmile}{_sushiSmile}{_sushiSmile} Sushi {_sushiSmile}{_sushiSmile}{_sushiSmile} ")
                .LeftAligned());
    }

    public void AddItemToTable(string value, int counter)
    {
        _table?.AddRow($"[blue]{counter}[/]", $"[white]{value}[/]");
        _table?.Border(TableBorder.Rounded);
    }

    public void PrintTableToConsole()
    {
        AnsiConsole.Write(_table!);
    }

    public void DisplayLoader()
    {
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Star)
            .Start("Processing...", ctx =>
            {
                // Omitted
            });
    }

    public string DisplayMakeChoice(string title, string[] variants)
    {
        var customerInput = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title(title)
            .AddChoices(variants)).ToLower();
        return customerInput;
    }

    public string DisplayMakeChoice(string title, List<Sushi> _sushi)
    {
        string customerInput = "";
        string[] sushiList = new string[_sushi.Count];
        for (int i = 0; i < _sushi.Count; i++)
        {
            sushiList[i] = $"Sushi {_sushi[i].Name}, {_sushi[i].Amount} items for {_sushi[i].Price}€";
        }

        customerInput = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title(title)
            .AddChoices(sushiList));
        return customerInput;
    }
}