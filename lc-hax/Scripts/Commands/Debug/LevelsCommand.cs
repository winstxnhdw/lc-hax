using System.Threading;
using System.Threading.Tasks;
using Hax;

[DebugCommand("levels")]
class LevelsCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        Helper.StartOfRound?.levels.ForEach((i, level) =>
            Logger.Write($"{level.name} = {i}")
        );
    }
}
