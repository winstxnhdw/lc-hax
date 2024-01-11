using Hax;
using System;
using System.Linq;

[Command("/visit")]
public class VisitCommand : ICommand {
    bool IsValidLevelIndex(string levelIndex, out ushort chosenLevelId) =>
        ushort.TryParse(levelIndex, out chosenLevelId) &&
        Enum.IsDefined(typeof(Level), chosenLevelId);

    bool TryParseLevel(string levelNameOrId, out int levelIndex) {
        if (int.TryParse(levelNameOrId, out int index)) {
            levelIndex = index;
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
