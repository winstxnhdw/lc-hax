using Hax;
using System;

[Command("steal")]
class StealSaveCommand : ICommand {
    public void Execute(StringArray args) {
        if (Helper.StartOfRound is not StartOfRound round) {
            return;
        }

        if (Helper.GameNetworkManager is not GameNetworkManager manager) {
            return;
        }


        // Check if the ship has landed
        if (round.shipHasLanded) {
            Console.WriteLine("Ship has landed. Aborting.");
            Helper.SendNotification(title: "Savegame Stealer", "Ship must be in orbit.", false);
            return;
        }

        string saveFileName = "";
        bool oldIsHostingGame = manager.isHostingGame;
        string alias = manager.steamLobbyName + " copy";
        string oldSaveFileName = manager.currentSaveFileName;
        int oldMaxItemCap = round.maxShipItemCapacity;
        string? newAlias = args[0];

        if (newAlias is not null and not "") {
            alias = newAlias;
        }

        Console.WriteLine($"Alias after check: {alias}");

        for (int i = 1; i < 17; i++) {
            if (!ES3.FileExists("LCSaveFile" + i)) {
                saveFileName = "LCSaveFile" + i;
                break;
            }
            else {
                if (ES3.KeyExists("Alias_BetterSaves", "LCSaveFile" + i)) {
                    if (ES3.Load<string>("Alias_BetterSaves", "LCSaveFile" + i) == alias) {
                        saveFileName = "LCSaveFile" + i;
                        break;
                    }
                }
            }
        }
        if (saveFileName == "") {
            Helper.SendNotification(title: "Savegame Stealer", "No Open Save Slot found.");
            return;
        }

        manager.isHostingGame = true;
        manager.currentSaveFileName = saveFileName;
        round.maxShipItemCapacity = 999;

        try {
            manager.SaveGame();
            ES3.Save("Alias_BetterSaves", alias, saveFileName);
        }
        catch (Exception e) {
            Logger.Write(e);
            Helper.SendNotification(title: "Savegame Stealer", "Saving Failed.", true);
            return; 
        }

        manager.isHostingGame = oldIsHostingGame;
        manager.currentSaveFileName = oldSaveFileName;
        round.maxShipItemCapacity = oldMaxItemCap;
        Helper.SendNotification(title: "Savegame Stealer", $"Save Successful with alias: {alias}.");
    }
}
