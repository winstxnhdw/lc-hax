using System;
using System.Linq;
using System.Collections.Generic;
using Hax;

[Command("suit")]
internal class SuitCommand : ICommand {
    internal Dictionary<string, Unlockable> SuitUnlockables =>
        Enum.GetValues(typeof(Unlockable))
            .Cast<Unlockable>()
            .Where(u => u.ToString().EndsWith("_SUIT"))
            .ToDictionary(suit => suit.ToString().Replace("_SUIT", "").ToLower(), suit => suit);

    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: suit <suit>");
            return;
        }

        string? key = Helper.FuzzyMatch(args[0], [.. this.SuitUnlockables.Keys]);

        if (key == null) {
            Chat.Print($"Suit is not found!");
            return;
        }

        Unlockable selectedSuit = this.SuitUnlockables[key];

        Helper
            .FindObjects<UnlockableSuit>()
            .First(suit => this.SuitUnlockables[key].Is(suit.suitID))?
            .SwitchSuitToThis(Helper.LocalPlayer);

        Chat.Print($"Wearing {string.Join(" ", selectedSuit.ToString().Split('_').Select(s => s.ToLower())).ToTitleCase()}!");
    }
}
