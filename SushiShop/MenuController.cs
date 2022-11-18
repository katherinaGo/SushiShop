using Newtonsoft.Json;
using Spectre.Console;
using SushiShop.ConsoleController;
using SushiShop.Texts;

namespace SushiShop;

public static class MenuController
{
    private const string MenuPathFile = "/Users/kate/RiderProjects/SushiShop/SushiShop/menu.json";

    public static List<Sushi> ParseMenuFromJson()
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

    public static void ShowMenu(List<Sushi> sushiMenu)
    {
        new ConsoleViewController();
        PrintToConsole(TextStrings.GetString(Keys.TodayInMenu));
        for (var j = 0; j < sushiMenu.Count; j++)
        {
            ConsoleViewController.AddItemToTable(sushiMenu[j].ToString(), j + 1);
        }

        ConsoleViewController.PrintTableToConsole();
    }

    public static void PrintToConsole(string text)
    {
        AnsiConsole.Write(new Markup(text));
    }
}