using System.Collections;
using System.Linq;
using UnityEngine;

namespace Hax;

public class ShovelMod : MonoBehaviour {
    bool IsLocalPlayerShovel(Shovel shovel) => shovel.playerHeldBy?.playerClientId == Helper.LocalPlayer?.playerClientId;

    Shovel? LocalPlayerShovel => HaxObject.Instance?.Shovels.Objects?.FirstOrDefault(this.IsLocalPlayerShovel);

    IEnumerator SetShovelForce() {
        while (true) {
            if (!Helper.Extant(this.LocalPlayerShovel, out Shovel shovel)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            shovel.shovelHitForce = Settings.ShovelHitForce;
            yield return new WaitForEndOfFrame();
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.SetShovelForce());
    }
}
