using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Hax;

public class ClearVisionMod : MonoBehaviour {
    IEnumerator SetClearVision() {
        HDAdditionalLightData? lightData = null;

        while (true) {
            if (!Helpers.Extant(TimeOfDay.Instance, out TimeOfDay timeOfDay)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            if (!Helpers.Extant(Helpers.StartOfRound, out StartOfRound startOfRound)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            if (timeOfDay.sunIndirect == null) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            if (lightData == null) {
                lightData = timeOfDay.sunIndirect.GetComponent<HDAdditionalLightData>();
                yield return new WaitForEndOfFrame();
                continue;
            }

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
