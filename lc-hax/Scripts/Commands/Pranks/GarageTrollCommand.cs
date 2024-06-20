#region

using System;
using GameNetcodeStuff;
using Hax;

#endregion

[Command("garagetroll")]
class GarageTrollCommand : ICommand {

    InteractTrigger? GarageTrigger => HaxObjects.Instance?.InteractTriggers?.WhereIsNotNull().First(
        interactTrigger => interactTrigger.name is "Cube" && interactTrigger.transform.parent.name is "Cutscenes"
    );

    public void Execute(StringArray args) {
        if (Helper.RoundManager is not RoundManager roundManager) return;
        if (roundManager.currentLevel.levelID is not 0) {
            Chat.Print("You must be in Experimentation to use this command!");
            return;
        }

        if (args.Length == 0) {
            Chat.Print($"Usage: garagetroll <on|off>");
            return;
        }

        string command = args[0].ToLower();
        switch (command) {
            case "on":
                Chat.Print("Garage troll is now enabled.");
                SetupCustomTrigger(true);
                break;
            case "off":
                Chat.Print("Garage troll is now disabled.");
                SetupCustomTrigger(false);
                break;
            default:
                Chat.Print($"Unknown command: {command}. Usage: garagetroll <on|off>");
                break;
        }
    }

    void TripGarage(PlayerControllerB player) {

        if (this.GarageTrigger is not InteractTrigger garageTrigger) {
            Chat.Print("Garage trigger is not found!");
            return;
        }

        if (player != null) {
            Console.WriteLine($"Player {player.GetPlayerUsername()} has tripped the garage!");
        }

        garageTrigger.randomChancePercentage = 100;
        garageTrigger.Interact(Helper.LocalPlayer?.transform);
    }

    void SetupCustomTrigger(bool Install) {
        if (Helper.RoundManager is not RoundManager roundManager) return;
        if (roundManager.currentLevel.levelID is not 0) {
            Chat.Print("You must be in Experimentation to use this command!");
            return;
        }

        if (this.GarageTrigger is not InteractTrigger garageTrigger) {
            Chat.Print("Garage trigger is not found!");
            return;
        }

        if (Install) {
            CustomPlayerTrigger trigger = garageTrigger.GetOrAddComponent<CustomPlayerTrigger>();
            trigger.onPlayerEnter += this.TripGarage;
        }
        else {
            garageTrigger.RemoveComponent<CustomPlayerTrigger>();
        }
    }
}
