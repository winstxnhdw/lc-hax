#region

using Hax;

#endregion

[Command("breakertroll")]
class BreakerTrollCommand : ICommand {
    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: breakertroll <on/off> [time]");
            return;
        }


        if (Helper.BreakerBox is not BreakerBox Breaker) {
            Chat.Print("Breaker box is not found!");
            return;
        }

        if (args[0].ToLower() == "on") {
            if (args.Length == 1) {
                Breaker.GetOrAddComponent<BreakerTrollMod>();
                Chat.Print("BreakerBox Troll Mod installed.");
                return;
            }

            if (float.TryParse(args[1], out float Timer)) {
                BreakerTrollMod? breakerTrollMod = Breaker.GetOrAddComponent<BreakerTrollMod>();
                breakerTrollMod.TimeBetweenSwitches = Timer;
                Chat.Print($"BreakerBox Troll Mod Installed & set with delay {Timer}.");
                return;
            }
            else {
                Chat.Print("Invalid value.");
                return;
            }
        }

        if (args[0].ToLower() == "off") {
            Breaker.RemoveComponent<BreakerTrollMod>();
            Chat.Print("BreakerBox Troll Mod uninstalled.");
        }
    }
}
