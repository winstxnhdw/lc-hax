using Hax;

[DebugCommand("/unlockables")]
public class UnlockablesCommand : ICommand {
    public void Execute(string[] _) {
        Helper.StartOfRound?.unlockablesList.unlockables.ForEach((i, unlockable) =>
            Logger.Write($"{unlockable.unlockableName} = {i}")
        );
    }
}
