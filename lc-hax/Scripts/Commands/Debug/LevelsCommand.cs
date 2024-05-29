using Hax;

[DebugCommand("levels")]
internal class LevelsCommand : ICommand
{
    public void Execute(StringArray _)
    {
        Helper.StartOfRound?.levels.ForEach((i, level) =>
            Logger.Write($"{level.name} = {i}")
        );
    }
}