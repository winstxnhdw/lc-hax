namespace Hax;

public class ScrapCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length < 1) {
            Console.Print("SYSTEM", "Usage: /scrap <value=-1>");
        }

        Settings.ScrapValue = int.TryParse(args[0], out int scrapValue) ? scrapValue : Settings.ScrapValue;
    }
}
