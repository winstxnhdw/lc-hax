#region

using System.Linq;
using Hax;

#endregion

[Command("suit")]
class SuitCommand : ICommand {
    public void Execute(StringArray args) {
        if (args[0] is not string name) {
            Chat.Print("Usage: suit <suit>");
            Chat.Print("Available suits: " + string.Join(", ", Helper.Suits.Keys.OrderBy(s => s)));
            return;
        }

        if (!name.FuzzyMatch(Helper.Suits.Keys, out string key)) {
            Chat.Print("Suit is not found!");
            Chat.Print("Available suits: " + string.Join(", ", Helper.Suits.Keys.OrderBy(s => s)));
            return;
        }

        int selectedSuit = Helper.Suits[key];
        Helper.BuyUnlockable(selectedSuit);

        UnlockableSuit suit = Helper.FindObjects<UnlockableSuit>().First(s => s.suitID == selectedSuit);
        if (suit is null) {
            Chat.Print("Failed to find a Spawned Suit instance!");
            return;
        }

        suit.SwitchSuitToThis(Helper.LocalPlayer);
        Chat.Print($"Wearing a  {key}!");
    }
}
