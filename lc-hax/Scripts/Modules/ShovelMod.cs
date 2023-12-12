using System.Collections;
using System.Linq;
using UnityEngine;

namespace Hax;

public class ShovelMod : MonoBehaviour {
    IEnumerator SetShovelForce() {
        while (true) {
            Shovel? localPlayerShovel =
                HaxObject.Instance?
                          .Shovels
                          .Objects?
                          .FirstOrDefault(shovel =>
                shovel.playerHeldBy.playerClientId == Helper.LocalPlayer?.playerClientId
            );

            if (localPlayerShovel == null) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            localPlayerShovel.shovelHitForce = Settings.ShovelHitForce;
            yield return new WaitForEndOfFrame();
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.SetShovelForce());
    }
}
