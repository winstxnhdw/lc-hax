using Hax;
using System.Linq;

[Command("trapshield")]
class TrapShieldCommand : ICommand {


    public void Execute(StringArray args) {
        if (args.Length is 0)
        {
            Chat.Print("Usage: trapshield <on/off> --all");
            return;
        }

        bool AllPlayers = args.Length > 1 && args[1].ToLower() == "--all";
        if (args[0].ToLower() == "on")
        {
            InstallTrapShield(AllPlayers);
            return;
        }

        else if (args[0].ToLower() == "off")
        {
            UninstallTrapShield();
            return;
        }
    }

    void InstallTrapShield(bool AllPlayers = false)
    {
        int Turrets = Helper.FindObjects<Turret>().Count();
        int SpikeRoofTraps = Helper.FindObjects<SpikeRoofTrap>().Count();
        int Landmines = Helper.FindObjects<Landmine>().Count();
        Helper.FindObjects<Turret>().ForEach(x=>
        {
            TrapControllerMod Controller = x.GetOrAddComponent<TrapControllerMod>();
            if (Controller != null)
            {
                Controller.OnlyForLocalPlayer = !AllPlayers;
            }
         });
        Helper.FindObjects<Landmine>().ForEach(x =>
        {
            TrapControllerMod Controller = x.GetOrAddComponent<TrapControllerMod>();
            if (Controller != null)
            {
                Controller.OnlyForLocalPlayer = !AllPlayers;
            }
        });
        Helper.FindObjects<SpikeRoofTrap>().ForEach(x =>
        {
            TrapControllerMod Controller = x.GetOrAddComponent<TrapControllerMod>();
            if (Controller != null)
            {
                Controller.OnlyForLocalPlayer = !AllPlayers;
            }
        });
        Chat.Print($"Trapshield Has been enabled for this round, for {Turrets} Turrets  , Landmines {Landmines} and  {SpikeRoofTraps} Spike Roof Traps");
        if(AllPlayers)
        {
            Chat.Print("Trapshield is enabled for all players.");
        }
        else
        {
            Chat.Print("Trapshield is enabled for local player only.");
        }
    }

    void UninstallTrapShield()
    {
        Helper.FindObjects<Turret>().ForEach(turret => turret.RemoveComponent<TrapControllerMod>());
        Helper.FindObjects<SpikeRoofTrap>().ForEach(spikeRoofTrap => spikeRoofTrap.RemoveComponent<TrapControllerMod>());
        Helper.FindObjects<Landmine>().ForEach(landmine => landmine.RemoveComponent<TrapControllerMod>());
        Chat.Print("Trapshield Has been disabled for this Level.");

    }
}
