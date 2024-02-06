using Hax;
using System;
using System.Collections.Generic;
using System.Linq;

[Command("/suit")]
internal class SuitCommand : ICommand {
    internal Dictionary<string, Unlockable> SuitUnlockables =>
        Enum.GetValues(typeof(Unlockable))
            .Cast<Unlockable>()
            .Where(u => u.ToString().EndsWith("_SUIT"))
            .ToDictionary(suit => suit.ToString().Replace("_SUIT", "").ToLower(), suit => suit);

    public void Execute(StringArray args) {
        if (args.Length == 0) {
            Chat.Print("Usage: /suit <name>");
            Chat.Print($"Available Suits: {string.Join(", ", this.SuitUnlockables.Keys)}");
            return;
        }

        string key = Helper.FuzzyMatch(args[0].ToLower(), [.. this.SuitUnlockables.Keys]);
        Unlockable unlockable = this.SuitUnlockables[key];
        Chat.Print($"Attempting to Wear a {string.Join(' ', unlockable.ToString().Split('_')).ToTitleCase()}!");
        UnlockableSuitPatch.SetPlayerSuit(unlockable);
    }
}
