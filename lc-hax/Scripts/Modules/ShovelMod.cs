using System.Linq;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class ShovelMod : MonoBehaviour {
    void Update() {
        PlayerControllerB? player = HaxObjects.Instance?.GameNetworkManager.Object?.localPlayerController;

        if (player == null) {
            return;
        }

        Shovel[]? shovels = HaxObjects.Instance?.Shovels.Objects;

        if (shovels == null) {
            return;
        }

        shovels.First(shovel => shovel.playerHeldBy.playerClientId == player.playerClientId).shovelHitForce = Settings.ShovelHitForce;
    }
}
