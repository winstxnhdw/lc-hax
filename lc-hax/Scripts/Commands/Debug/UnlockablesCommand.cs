using Hax;

[DebugCommand("unlockables")]
class UnlockablesCommand : ICommand {
    public void Execute(StringArray _) {
        Helper.StartOfRound?.unlockablesList.unlockables.ForEach((i, unlockable) =>
            Logger.Write($"{unlockable.unlockableName} = {i}")
        );
    }
}
