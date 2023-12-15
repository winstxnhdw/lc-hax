using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Hax;

public class ClearVisionMod : MonoBehaviour {
    IEnumerator SetClearVision() {
        HDAdditionalLightData? lightData = null;

        while (true) {
            if (!Helper.StartOfRound.IsNotNull(out StartOfRound startOfRound) ||
                !TimeOfDay.Instance.IsNotNull(out TimeOfDay timeOfDay) ||
                !timeOfDay.sunIndirect.IsNotNull(out Light sunIndirect)
            ) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            timeOfDay.sunAnimator.enabled = false;
            sunIndirect.transform.eulerAngles = new Vector3(90, 0, 0);
            timeOfDay.sunDirect.transform.eulerAngles = new Vector3(90, 0, 0);
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
