using System;

namespace Hax;

public class VisitCommand : ICommand {
    bool IsValidLevelIndex(string levelIndex, out int chosenLevelId) =>
        int.TryParse(levelIndex, out chosenLevelId) && 
        Enum.IsDefined(typeof(Level), chosenLevelId);

    bool TryParseLevel(string levelNameOrId, out int levelIndex) {
        if (this.IsValidLevelIndex(levelNameOrId, out int chosenLevelId)) {
            levelIndex = chosenLevelId;
            return true;
        }

        if (Enum.TryParse(levelNameOrId, true, out Level levelEnum)) {
            levelIndex = (int)levelEnum;
            return true;
        }

        levelIndex = -1;
        return false;
    }

    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /visit <moon>");
            return;
        }

        if (!Helper.Terminal.IsNotNull(out Terminal terminal)) {
            Console.Print("Terminal not found!");
            return;
        }

        if (!this.TryParseLevel(args[0], out int levelIndex)) {
            Console.Print("Invalid level!");
            return;
        }

        Helper.StartOfRound?.ChangeLevelServerRpc(levelIndex, terminal.groupCredits);
    }
}
