using System.Collections;
using UnityEngine;

namespace Hax;

public class BuildAnywhereMod : MonoBehaviour {
    IEnumerator SetCanConfirmPosition() {
        while (true) {
            _ = Helper.ShipBuildModeManager?.Reflect().SetInternalField("CanConfirmPosition", true);
            yield return new WaitForEndOfFrame();
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.SetCanConfirmPosition());
    }
}
