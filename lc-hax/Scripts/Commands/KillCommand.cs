using System.Linq;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class KillCommand : ICommand {
    PlayerControllerB[]? Players => HaxObjects.Instance?.Players.Objects;

    PlayerControllerB? GetPlayer(string name) => this.Players?.FirstOrDefault(player => player.playerUsername == name);

    public void Execute(string[] args) {
        if (args.Length < 1) {
            Console.Print("SYSTEM", "Usage: /kill <player>");
            return;
        }

        if (args[0] is "all") {
            this.Players.ToList()
                        .ForEach(player => player.DamagePlayerFromOtherClientServerRpc(1000, Vector3.zero, -1));
        }

        else {
            PlayerControllerB? targetPlayer = this.GetPlayer(args[0]);

            if (targetPlayer == null) {
                Console.Print("SYSTEM", "Player not found!");
                return;
            }

            targetPlayer.DamagePlayerFromOtherClientServerRpc(1000, Vector3.zero, -1);
            Console.Print("SYSTEM", $"Attempting to kill {targetPlayer.playerUsername}!");
        }
    }
}
