using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

[DebugCommand("mute")]
internal class MuteCommand : ICommand
{
    private static Dictionary<string, TransientBehaviour> MutedPlayers { get; } = [];

    public void Execute(StringArray args)
    {
        if (args.Length is 0)
        {
            Chat.Print("Usage: mute <player>");
            return;
        }

        if (Helper.GetPlayer(args[0]) is not PlayerControllerB player)
        {
            Chat.Print("Player is not alive or found!");
            return;
        }

        if (MutedPlayers.TryGetValue(player.playerUsername, out var existingMuterObject))
        {
            Object.Destroy(existingMuterObject);
            _ = MutedPlayers.Remove(player.playerUsername);
            Chat.Print($"Unmuted {player.playerUsername}!");
            return;
        }

        var playerMuterObject = Helper.CreateComponent<TransientBehaviour>().Init(_ =>
        {
            if (!MutedPlayerHasMessaged(player.playerUsername)) return;
            Chat.Clear();
        }, 10000.0f);

        MutedPlayers.Add(player.playerUsername, playerMuterObject);
        Chat.Print($"Muted {player.playerUsername}!");
    }

    private bool MutedPlayerHasMessaged(string playerUsername)
    {
        return Helper.HUDManager?.ChatMessageHistory.First(message =>
            message.StartsWith($"<color=#FF0000>{playerUsername}<color>: <color=#FFFF00>'"
            )) is not null;
    }
}