using System.Collections.Generic;

namespace Hax;

public class DebugCommand : ICommand {
    public void Execute(string[] args) {
        if (!Helper.StartOfRound.IsNotNull(out StartOfRound startOfRound)) return;

        List<UnlockableItem> unlockables = startOfRound.unlockablesList.unlockables;

        for (int i = 0; i < unlockables.Count; i++) {
            Logger.Write($"{unlockables[i].unlockableName}: {i}");
        }
    }
}
