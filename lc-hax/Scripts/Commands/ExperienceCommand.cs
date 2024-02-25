using Hax;

enum Rank {
    INTERN,
    PART_TIME,
    EMPLOYEE,
    LEADER,
    BOSS
}

[Command("xp")]
class ExperienceCommand : ICommand {
    public void Execute(StringArray args) {
        if (Helper.HUDManager is not HUDManager hudManager) return;
        if (args.Length is 0) {
            Chat.Print("Usage: xp <amount>");
            return;
        }

        if (!int.TryParse(args[0], out int amount)) {
            Chat.Print("Invalid amount!");
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

        hudManager.localPlayerLevel = unchecked((int)rank);

        ES3.Save("PlayerXPNum", hudManager.localPlayerXP, "LCGeneralSaveData");
        ES3.Save("PlayerLevel", hudManager.localPlayerLevel, "LCGeneralSaveData");

        hudManager.SyncPlayerLevelServerRpc(
            hudManager.localPlayer.PlayerIndex(),
            hudManager.localPlayerLevel,
            ES3.Load("playedDuringBeta", "LCGeneralSaveData", true)
        );

        Chat.Print($"You are a {rank} with {hudManager.localPlayerXP} XP!");
    }
}
