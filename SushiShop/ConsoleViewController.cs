using Spectre.Console;

namespace SushiShop;

public class ConsoleViewController
{
    private static Table? _table;
    private const string SushiSmile = ":sushi:";

    public ConsoleViewController()
    {
        _table = new Table();
        _table.Title("[blue]MENU[/]");
        _table.AddColumn("#");
        _table.AddColumn(
            new TableColumn($"{SushiSmile}{SushiSmile}{SushiSmile} Sushi {SushiSmile}{SushiSmile}{SushiSmile} ")
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

    public string DisplayMakeChoice(string title, string[] variants)
    {
        var customerInput = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title(title)
            .AddChoices(variants)).ToLower();
        return customerInput;
    }

    public string DisplayMakeChoice(string title, List<Sushi> sushi)
    {
        string[] sushiList = new string[sushi.Count];
        for (int i = 0; i < sushi.Count; i++)
        {
            sushiList[i] = $"Sushi {sushi[i].Name}, {sushi[i].AmountInPortion} items for {sushi[i].Price}€";
        }

        string customerInput = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title(title)
            .AddChoices(sushiList));
        return customerInput;
    }
}