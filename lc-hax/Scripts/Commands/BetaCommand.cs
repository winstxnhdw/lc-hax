using Hax;

[Command("beta")]
class BetaCommand : ICommand {
    public void Execute(StringArray _) {
        bool playedDuringBeta = ES3.Load("playedDuringBeta", "LCGeneralSaveData", true);
        ES3.Save("playedDuringBeta", !playedDuringBeta, "LCGeneralSaveData");
        Chat.Print($"Beta badge: {(!playedDuringBeta ? "obtained" : "removed")}");
    }
}
