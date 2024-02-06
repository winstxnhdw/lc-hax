using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

[DebugCommand("/mute")]
internal class MuteCommand : ICommand {
    static Dictionary<string, TransientBehaviour> MutedPlayers { get; } = [];

    bool MutedPlayerHasMessaged(string playerUsername) =>
        Helper.HUDManager?.ChatMessageHistory.First(message =>
            message.StartsWith($"<color=#FF0000>{playerUsername}</color>: <color=#FFFF00>'"
        )) is not null;

    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /mute <player>");
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
            if (!this.MutedPlayerHasMessaged(player.playerUsername)) return;

            string newChatText = string.Join('\n', Helper.HUDManager?.ChatMessageHistory.Where(message =>
                message.StartsWith($"<color=#FF0000>{player.playerUsername}</color>: <color=#FFFF00>'"
            )));

            Chat.Announce(newChatText, true);
        }, 10000.0f);

        MuteCommand.MutedPlayers.Add(player.playerUsername, playerMuterObject);
        Chat.Print($"Muted {player.playerUsername}!");
    }
}
