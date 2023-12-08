using System.Collections;
using System.Linq;
using UnityEngine;

namespace Hax;

public class ShovelMod : MonoBehaviour {
    IEnumerator SetShovelForce() {
        while (true) {
            Shovel? localPlayerShovel =
                HaxObjects.Instance?
                          .Shovels
                          .Objects?
                          .FirstOrDefault(shovel =>
                shovel.playerHeldBy.playerClientId == Helpers.LocalPlayer?.playerClientId
            );

            if (localPlayerShovel == null) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            localPlayerShovel.shovelHitForce = Settings.ShovelHitForce;
            yield return new WaitForSeconds(1.0f);
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.SetShovelForce());
    }
}
