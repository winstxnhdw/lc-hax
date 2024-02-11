using System.Linq;
using System.Collections.Generic;
using Hax;

[Command("build")]
internal class BuildCommand : ICommand {
    public void Execute(StringArray args) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.CurrentCamera is not Camera camera) return;
        if (args.Length is 0) {
            Chat.Print("Usage: build <unlockable>");
            return;
        }

        Dictionary<string, int> unlockables =
            startOfRound.unlockablesList.unlockables.Select((unlockable, i) => (unlockable, i)).ToDictionary(
                pair => pair.unlockable.unlockableName.ToLower(),
                pair => pair.i
            );

        string? key = Helper.FuzzyMatch(args[0]?.ToLower(), unlockables.Keys);

        if (string.IsNullOrWhiteSpace(key)) {
            Chat.Print("Failed to find unlockable!");
            return;
        }

        Unlockable unlockable = (Unlockable)unlockables[key];
        Helper.BuyUnlockable(unlockable);
        Helper.ReturnUnlockable(unlockable);

        Chat.Print($"Attempting to build a {string.Join(' ', unlockable.ToString().Split('_')).ToTitleCase()}!");

        if (Helper.GetUnlockable(unlockable) is not PlaceableShipObject shipObject) {
            Chat.Print("Unlockable is not found or placeable!");
            return;
        }

        Vector3 newPosition = camera.transform.position + (camera.transform.forward * 3.0f);
        Vector3 newRotation = camera.transform.eulerAngles;
        newRotation.x = -90.0f;

        Helper.PlaceObjectAtPosition(
            shipObject,
            newPosition,
            newRotation
        );
    }
}
