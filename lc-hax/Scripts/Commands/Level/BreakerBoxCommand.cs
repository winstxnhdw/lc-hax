using Hax;
using System.Linq;

[Command("breaker")]
class BreakerBoxCommand : ICommand {


    public void Execute(StringArray args) {
        if (args.Length is 0)
        {
            Chat.Print("Usage: breaker <on/off> [all/index 1-5]");
            return;
        }

        BreakerBox Breaker = Helper.FindObjects<BreakerBox>().FirstOrDefault(breaker => breaker.name.Contains("(Clone)"));

        if (Breaker == null) {
            Chat.Print("Breaker box is not found!");
            return;
        }

        if (args[0].ToLower() == "on")
        {
            if (args.Length == 1 || args[1].ToLower() == "all")
            {
                Breaker.RemoveComponent<BreakerTrollMod>();
                Breaker.Set_All_BreakerBox_Switches(true);
                Chat.Print("All BreakerBox switches turned on.");
                return;
            }

            if (int.TryParse(args[1], out int index) && index >= 1 && index <= 5)
            {
                Breaker.RemoveComponent<BreakerTrollMod>();
                Breaker.Set_BreakerBox_Switch(index - 1, true);
                Chat.Print($"BreakerBox switch {index} turned on.");
                return;
            }
            else
            {
                Chat.Print("Invalid index. Please provide a number between 1 and 5, or 'all'.");
                return;
            }
        }

        if (args[0].ToLower() == "off")
        {
            if (args.Length == 1 || args[1].ToLower() == "all")
            {
                Breaker.RemoveComponent<BreakerTrollMod>();
                Breaker.Set_All_BreakerBox_Switches(false);
                Chat.Print("All BreakerBox switches turned off.");
                return;
            }

            if (int.TryParse(args[1], out int index) && index >= 1 && index <= 5)
            {
                Breaker.RemoveComponent<BreakerTrollMod>();
                Breaker.Set_BreakerBox_Switch(index - 1, false);
                Chat.Print($"BreakerBox switch {index} turned off.");
                return;
            }
            else
            {
                Chat.Print("Invalid index. Please provide a number between 1 and 5, or 'all'.");
                return;
            }
        }
    }
}
