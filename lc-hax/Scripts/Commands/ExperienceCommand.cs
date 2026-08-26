using System.Threading;
using System.Threading.Tasks;

enum Rank {
    INTERN,
    PART_TIME,
    EMPLOYEE,
    LEADER,
    BOSS
}

[Command("xp")]
sealed class ExperienceCommand : ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.HUDManager is not HUDManager hudManager)
            return Task.CompletedTask;
        if (args.Length is 0) {
            Chat.Print("Usage: xp <amount>");
            return Task.CompletedTask;
        }

        if (!int.TryParse(args[0], out int amount)) {
            Chat.Print($"Invalid {nameof(amount)}!");
            return Task.CompletedTask;
        }

        Rank rank = (hudManager.localPlayerXP += amount) switch {
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
        return Task.CompletedTask;
    }
}
