using System.Threading;
using System.Threading.Tasks;

[Command("beta")]
sealed class BetaCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        bool playedDuringBeta = ES3.Load("playedDuringBeta", "LCGeneralSaveData", true);
        ES3.Save("playedDuringBeta", !playedDuringBeta, "LCGeneralSaveData");
        Chat.Print($"Beta badge: {(!playedDuringBeta ? "obtained" : "removed")}");
    }
}
