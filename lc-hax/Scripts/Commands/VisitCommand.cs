using System;

namespace Hax;

public class VisitCommand : ICommand {
    bool TryParseLevel(string levelNameOrId, out int levelId) {
        if (int.TryParse(levelNameOrId, out int chosenLevelId) && Enum.IsDefined(typeof(Level), chosenLevelId)) {
            levelId = chosenLevelId;
            return true;
        }

        if (Enum.TryParse(levelNameOrId, true, out Level levelEnum)) {
            levelId = (int)levelEnum;
            return true;
        }

        levelId = -1;
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

        if (!this.TryParseLevel(args[0], out int levelId)) {
            Console.Print("Invalid level!");
            return;
        }

        Helper.StartOfRound?.ChangeLevelServerRpc(levelId, terminal.groupCredits);
    }
}
