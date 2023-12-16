using System.Collections;
using UnityEngine;

namespace Hax;

public class BuildAnywhereMod : MonoBehaviour {
    IEnumerator SetCanConfirmPosition() {
        while (true) {
            if (!Helper.ShipBuildModeManager.IsNotNull(out ShipBuildModeManager shipBuildModeManager)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            _ = Reflector.Target(shipBuildModeManager).SetInternalField("CanConfirmPosition", true);
            yield return new WaitForEndOfFrame();
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.SetCanConfirmPosition());
    }
}
