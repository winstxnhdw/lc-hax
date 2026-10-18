using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

[DebugCommand("mute")]
sealed class MuteCommand : ICommand {
    static Dictionary<ulong, CancellationTokenSource> MutedPlayers { get; } = [];

    async Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.StartOfRound is not { } round) return;
        if (Helper.HUDManager is not { ChatMessageHistory: { } chatHistory }) return;
        if (args.Length is 0) {
            Chat.Print("Usage: mute <player>");
            return;
        }

        if (Helper.GetPlayer(args[0]) is not { actualClientId: ulong clientId } player) {
            Chat.Print("Player not found!");
            return;
        }

        if (MuteCommand.MutedPlayers.TryGetValue(clientId, out CancellationTokenSource existingCancellation)) {
            existingCancellation.Cancel();
            _ = MuteCommand.MutedPlayers.Remove(clientId);
            Chat.Print($"Unmuted {player.playerUsername}!");
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();
        using CancellationTokenSource muteToken = new();
        MuteCommand.MutedPlayers.Add(clientId, muteToken);
        Chat.Print($"Muted {player.playerUsername}!");
        string muteMessagePrefix = $"<color=#FF0000>{player.playerUsername}<color>: <color=#FFFF00>'";

        while (round.ClientPlayerList.ContainsKey(clientId)) {
            if (chatHistory.First(message => message.StartsWith(muteMessagePrefix)) is not null) {
                Chat.Clear();
            }

            await Task.Delay(100, muteToken.Token);
        }
    }
}
