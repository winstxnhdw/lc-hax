using System;
using Hax;

[Command("/beta")]
public class BetaCommand : ICommand {
    public void Execute(ReadOnlySpan<string> _) {
        bool playedDuringBeta = ES3.Load("playedDuringBeta", "LCGeneralSaveData", true);
        ES3.Save("playedDuringBeta", !playedDuringBeta, "LCGeneralSaveData");
        Chat.Print($"Beta badge: {(!playedDuringBeta ? "obtained" : "removed")}");
    }
}
