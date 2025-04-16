using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;

[DebugCommand("mute")]
class MuteCommand : ICommand {
    static Dictionary<string, TransientBehaviour> MutedPlayers { get; } = [];

    static bool MutedPlayerHasMessaged(string playerUsername) =>
        Helper.HUDManager?.ChatMessageHistory.First(message =>
            message.StartsWith($"<color=#FF0000>{playerUsername}<color>: <color=#FFFF00>'"
        )) is not null;

    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: mute <player>");
            return;
        }

        if (Helper.GetPlayer(args[0]) is not PlayerControllerB player) {
            Chat.Print("Player is not alive or found!");
            return;
        }

        if (MuteCommand.MutedPlayers.TryGetValue(player.playerUsername, out TransientBehaviour existingMuterObject)) {
            Object.Destroy(existingMuterObject);
            _ = MuteCommand.MutedPlayers.Remove(player.playerUsername);
            Chat.Print($"Unmuted {player.playerUsername}!");
            return;
        }

        TransientBehaviour playerMuterObject = Helper.CreateComponent<TransientBehaviour>().Init(_ => {
            if (!MuteCommand.MutedPlayerHasMessaged(player.playerUsername)) return;
            Chat.Clear();
        }, 10000.0f);

        MuteCommand.MutedPlayers.Add(player.playerUsername, playerMuterObject);
        Chat.Print($"Muted {player.playerUsername}!");
    }
}
