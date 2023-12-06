using System.Linq;
using UnityEngine;

namespace Hax;

public class ShovelMod : MonoBehaviour {
    void Update() {
        Shovel? localPlayerShovel =
            HaxObjects.Instance?
                      .Shovels
                      .Objects?
                      .FirstOrDefault(shovel =>
            shovel.playerHeldBy.playerClientId == Helpers.LocalPlayer?.playerClientId
        );

        if (localPlayerShovel == null) {
            return;
        }

        localPlayerShovel.shovelHitForce = Settings.ShovelHitForce;
    }
}
