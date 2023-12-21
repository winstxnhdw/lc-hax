using System.Collections;
using System.Linq;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public sealed class ShovelMod : MonoBehaviour {
    PlayerControllerB? ShovelOwner(Shovel shovel) => shovel.playerHeldBy;

    bool IsLocalPlayerShovel(Shovel shovel) =>
        Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer) &&
        !this.ShovelOwner(shovel).IsNotNull(out PlayerControllerB shovelOwner) &&
        shovelOwner.actualClientId == localPlayer.actualClientId;

    Shovel? LocalPlayerShovel => HaxObjects.Instance?.Shovels.Objects?.FirstOrDefault(this.IsLocalPlayerShovel);

    IEnumerator SetShovelForce() {
        while (true) {
            if (!this.LocalPlayerShovel.IsNotNull(out Shovel shovel)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            shovel.shovelHitForce = Settings.ShovelHitForce;
            yield return new WaitForSeconds(1.0f);
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.SetShovelForce());
    }
}
