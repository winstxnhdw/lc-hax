using Hax;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        string key = Helper.FuzzyMatch(args[0], this.SuitUnlockables.Keys.ToArray());

        if (!this.SuitUnlockables.ContainsKey(key)) {
            Chat.Print($"Suit '{args[0]}' not found. Please check the available suits.");
            return;
        }

        Unlockable selectedSuit = this.SuitUnlockables[key];
        List<UnlockableSuit> availableSuits = Helper.FindObjects<UnlockableSuit>().ToList();

        UnlockableSuit? matchingSuit = availableSuits.FirstOrDefault(suit => suit.suitID == (int)selectedSuit);

        if (matchingSuit != null) {
            matchingSuit.SwitchSuitToThis(Helper.LocalPlayer);
        }
        else {
            UnlockableSuit? randomSuit = availableSuits.First();
            if (randomSuit == null) {
                Chat.Print("No available suits found.");
                return;
            }

            int id = randomSuit.suitID;
            randomSuit.suitID = (int)selectedSuit;
            randomSuit.syncedSuitID.Value = (int)selectedSuit;

            // Apply the suit change
            randomSuit.SwitchSuitToThis(Helper.LocalPlayer);

            Helper.Delay(delay: 0.5f, action: () => {
                randomSuit.suitID = id;
                randomSuit.syncedSuitID.Value = id;
            });
        }

        Chat.Print($"Wearing: {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(string.Join(" ", selectedSuit.ToString().Split('_').Select(s => s.ToLower())))}!");
    }
}
