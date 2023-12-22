using System.Collections.Generic;

namespace Hax;

public class UnlockablesCommand : IDebugCommand {
    public void DebugExecute(string[] _) {
        if (!Helper.StartOfRound.IsNotNull(out StartOfRound startOfRound)) {
            Console.Print("StartOfRound not found!");
            return;
        }

        List<UnlockableItem> unlockables = startOfRound.unlockablesList.unlockables;

        for (int i = 0; i < unlockables.Count; i++) {
            Logger.Write($"{unlockables[i].unlockableName} = {i}");
        }
    }
}
