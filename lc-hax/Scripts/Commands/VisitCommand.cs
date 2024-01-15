using Hax;
using System;
using System.Linq;

[Command("/visit")]
public class VisitCommand : ICommand {
    bool IsValidLevelIndex(string levelIndex, out int chosenLevelId) =>
        Enum.TryParse(levelIndex, out chosenLevelId) &&
        Enum.IsDefined(typeof(Level), chosenLevelId);

    bool TryParseLevel(string levelNameOrId, out int levelIndex) {
        if (int.TryParse(levelNameOrId, out _)) {
            return this.IsValidLevelIndex(levelNameOrId, out levelIndex);
        }

        if (Enum.TryParse(levelNameOrId, true, out Level _)) {
            return this.IsValidLevelIndex(levelNameOrId, out levelIndex);
        }

        levelIndex = -1;
        return false;
    }

    public void Execute(string[] args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /visit <moon>");
            return;
        }

        if (!Helper.Terminal.IsNotNull(out Terminal terminal)) {
            Chat.Print("Terminal not found!");
            return;
        }

        if (!this.TryParseLevel(args[0], out int levelIndex)) {
            Chat.Print("Invalid level!");
            string levels = Enum.GetValues(typeof(Level)).Cast<Level>().Aggregate(string.Empty, (current, level) => current + $"{level} : {(int)level}");
            Chat.Print(levels);
            return;
        }

        Helper.StartOfRound?.ChangeLevelServerRpc(levelIndex, terminal.groupCredits);
    }
}
