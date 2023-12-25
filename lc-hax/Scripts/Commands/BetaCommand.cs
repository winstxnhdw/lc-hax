namespace Hax;

public class BetaCommand : ICommand {
    public void Execute(string[] args) {
        bool playedDuringBeta = ES3.Load("playedDuringBeta", "LCGeneralSaveData", true);

        ES3.Save(
            "playedDuringBeta",
            !playedDuringBeta,
            "LCGeneralSaveData"
        );

        Console.Print($"Beta badge: {(playedDuringBeta ? "obtained" : "removed")}");
    }
}
