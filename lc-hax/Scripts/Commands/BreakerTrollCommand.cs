using Hax;
using System.Linq;

[Command("breakertroll")]
internal class BreakerTrollCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (args.Length is 0)
        {
            Chat.Print("Usage: breakertroll <on/off> [time]");
            return;
        }

        BreakerBox[] Breakers = Helper.FindObjects<BreakerBox>().Where(breaker => breaker.name.Contains("(Clone)")).ToArray();

        if (Breakers.Count() is 0)
        {
            Chat.Print("Breaker box is not found!");
            return;
        }

        if (args[0].ToLower() == "on")
        {
            if (args.Length == 1)
            {
                Breakers.ForEach(breaker => breaker.GetOrAddComponent<BreakerTrollMod>());
                Chat.Print("BreakerBox Troll Mod installed.");
                return;
            }

            if (float.TryParse(args[1], out float Timer))
            {
                Breakers.ForEach(breaker =>
                {
                    BreakerTrollMod breakerTrollMod = breaker.GetOrAddComponent<BreakerTrollMod>();
                    breakerTrollMod.TimeBetweenSwitches = Timer;
                }
                );
                Chat.Print($"BreakerBox Troll Mod Installed & set with delay {Timer}.");
                return;
            }
            else
            {
                Chat.Print("Invalid value.");
                return;
            }
        }

        if (args[0].ToLower() == "off")
        {
            Breakers.ForEach(breaker => breaker.RemoveComponent<BreakerTrollMod>());
            Chat.Print("BreakerBox Troll Mod uninstalled.");
        }
    }
}