using Hax;
using System.Linq;

[Command("breaker")]
class BreakerBoxCommand : ICommand {


    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: breaker <on/off>");
            return;
        }
        BreakerBox[] Breakers = Helper.FindObjects<BreakerBox>();

        if (Breakers.Count() is 0) {
            Chat.Print("Breaker box is not found!");
            return;
        }

        if (args[0] is "on") {
            Breakers.ForEach(breaker => breaker.SwitchBreaker(true));
        }
        else if (args[0] is "off") {
            Breakers.ForEach(breaker => breaker.SwitchBreaker(false));
        }
        else {
            Chat.Print("Invalid property!");
            return;
        }
    }
}
