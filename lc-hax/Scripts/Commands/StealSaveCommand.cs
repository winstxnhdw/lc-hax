using System;
using Hax;

[Command("steal")]
internal class StealSaveCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (Helper.StartOfRound is not StartOfRound round) return;

        if (Helper.GameNetworkManager is not GameNetworkManager manager) return;


        // Check if the ship has landed
        if (round.shipHasLanded)
        {
            Console.WriteLine("Ship has landed. Aborting.");
            Helper.DisplayFlatHudMessage("Savegame Stealer : Ship must be in orbit.");
            return;
        }

        var saveFileName = "";
        var oldIsHostingGame = manager.isHostingGame;
        var alias = manager.steamLobbyName + " copy";
        var oldSaveFileName = manager.currentSaveFileName;
        var oldMaxItemCap = round.maxShipItemCapacity;
        var newAlias = args[0];

        if (newAlias is not null and not "") alias = newAlias;

        Console.WriteLine($"Alias after check: {alias}");

        for (var i = 1; i < 17; i++)
            if (!ES3.FileExists("LCSaveFile" + i))
            {
                saveFileName = "LCSaveFile" + i;
                break;
            }
            else
            {
                if (ES3.KeyExists("Alias_BetterSaves", "LCSaveFile" + i))
                    if (ES3.Load<string>("Alias_BetterSaves", "LCSaveFile" + i) == alias)
                    {
                        saveFileName = "LCSaveFile" + i;
                        break;
                    }
            }

        if (saveFileName == "")
        {
            Helper.DisplayFlatHudMessage("Savegame Stealer : No Open Save Slot found.");
            return;
        }

        manager.isHostingGame = true;
        manager.currentSaveFileName = saveFileName;
        round.maxShipItemCapacity = 999;

        try
        {
            manager.SaveGame();
            ES3.Save("Alias_BetterSaves", alias, saveFileName);
        }
        catch (Exception e)
        {
            Logger.Write(e);
            Helper.DisplayFlatHudMessage("Savegame Stealer : Saving Failed.");
            return;
        }

        manager.isHostingGame = oldIsHostingGame;
        manager.currentSaveFileName = oldSaveFileName;
        round.maxShipItemCapacity = oldMaxItemCap;
        Helper.DisplayFlatHudMessage($"Savegame Stealer : Save Successful with alias: {alias}.");
    }
}