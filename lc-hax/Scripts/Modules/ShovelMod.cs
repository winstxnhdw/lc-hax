using System.Collections;
using System.Linq;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class ShovelMod : MonoBehaviour {
    PlayerControllerB? ShovelOwner(Shovel shovel) => shovel.playerHeldBy;

    bool IsLocalPlayerShovel(Shovel shovel) => this.ShovelOwner(shovel)?.actualClientId == Helper.LocalPlayer?.actualClientId;

    Shovel? LocalPlayerShovel => HaxObject.Instance?.Shovels.Objects?.FirstOrDefault(this.IsLocalPlayerShovel);

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
