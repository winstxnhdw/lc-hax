namespace Hax;

public class LevelsCommand : ICommand {
    public void Execute(string[] _) {
        if (!Helper.StartOfRound.IsNotNull(out StartOfRound startOfRound)) {
            Console.Print("StartOfRound not found!");
            return;
        }

        startOfRound.levels.ForEach((i, level) => {
            Logger.Write($"{level.name} = {i}");
        });
    }
}
