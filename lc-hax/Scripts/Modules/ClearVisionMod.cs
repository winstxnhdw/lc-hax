using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Hax;

public class ClearVisionMod : MonoBehaviour {
    IEnumerator SetClearVision() {
        HDAdditionalLightData? lightData = null;
        while (true) {
            if (!Helper.Extant(TimeOfDay.Instance, out TimeOfDay timeOfDay)
                || !Helper.Extant(timeOfDay.sunIndirect, out Light sunIndirect)
                || !Helper.Extant(Helper.StartOfRound, out StartOfRound startOfRound)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            lightData ??= sunIndirect.GetComponent<HDAdditionalLightData>();
            lightData.lightDimmer = float.MaxValue;
            lightData.distance = float.MaxValue;
            timeOfDay.insideLighting = false;
            startOfRound.blackSkyVolume.weight = 0;

            yield return new WaitForEndOfFrame();
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.SetClearVision());
    }
}
