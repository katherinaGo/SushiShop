using Spectre.Console;

namespace SushiShop;

public class ConsoleViewController
{
    private static Table? _table;

    public ConsoleViewController()
    {
        _table = new Table();
        _table.Title("[blue]MENU[/]");
        _table.AddColumn("#");
        _table.AddColumn(new TableColumn("Sushi").LeftAligned());
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
}