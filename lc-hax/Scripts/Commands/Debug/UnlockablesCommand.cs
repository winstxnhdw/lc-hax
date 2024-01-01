namespace Hax;

[DebugCommand("/unlockables")]
public class UnlockablesCommand : ICommand {
    public void Execute(string[] _) {
        if (!Helper.StartOfRound.IsNotNull(out StartOfRound startOfRound)) {
            Console.Print("StartOfRound not found!");
            return;
        }

        startOfRound.unlockablesList.unlockables.ForEach((i, unlockable) => {
            Logger.Write($"{unlockable.unlockableName} = {i}");
        });
    }
}
