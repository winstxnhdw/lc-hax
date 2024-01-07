using Hax;

[Command("/xp")]
public class ExperienceCommand : ICommand {
    enum Rank {
        INTERN = 0,
        PART_TIME = 1,
        EMPLOYEE = 2,
        LEADER = 3,
        BOSS = 4
    }

    public void Execute(string[] args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /xp <amount>");
            return;
        }

        if (!int.TryParse(args[0], out int amount)) {
            Chat.Print("Invalid amount!");
            return;
        }

        if (!Helper.HUDManager.IsNotNull(out HUDManager hudManager)) {
            Chat.Print("HUDManager is not found");
            return;
        }

        hudManager.localPlayerXP += amount;

        Rank rank = hudManager.localPlayerXP switch {
            < 50 => Rank.INTERN,
            < 100 => Rank.PART_TIME,
            < 200 => Rank.EMPLOYEE,
            < 500 => Rank.LEADER,
            _ => Rank.BOSS
        };

        hudManager.localPlayerLevel = (int)rank;

        ES3.Save("PlayerXPNum", hudManager.localPlayerXP, "LCGeneralSaveData");
        ES3.Save("PlayerLevel", hudManager.localPlayerLevel, "LCGeneralSaveData");

        hudManager.SyncPlayerLevelServerRpc(
            (int)hudManager.localPlayer.playerClientId,
            hudManager.localPlayerLevel,
            ES3.Load("playedDuringBeta", "LCGeneralSaveData", true)
        );
    }
}
