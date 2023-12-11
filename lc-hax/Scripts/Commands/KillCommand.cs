using System.Linq;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class KillCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length < 1) {
            if (!Helper.Extant(Helper.LocalPlayer, out PlayerControllerB localPlayer)) {
                Helper.PrintSystem("Player not found!");
                return;
            }

            localPlayer.DamagePlayerFromOtherClientServerRpc(int.MaxValue, Vector3.zero, -1);
            return;
        }

        if (args[0] is "--all") {
            Helper.Players
                   .ToList()
                   .ForEach(player => player.DamagePlayerFromOtherClientServerRpc(int.MaxValue, Vector3.zero, -1));

            Helper.PrintSystem("Attempting to kill all players!");
            return;
        }

        if (!Helper.Extant(Helper.GetPlayer(args[0]), out PlayerControllerB targetPlayer)) {
            Helper.PrintSystem("Player not found!");
            return;
        }

        targetPlayer.DamagePlayerFromOtherClientServerRpc(int.MaxValue, Vector3.zero, -1);
        Helper.PrintSystem($"Attempting to kill {targetPlayer.playerUsername}!");
    }
}
