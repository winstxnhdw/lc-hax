using System.Collections;
using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Hax;

public class NightVisionMod : MonoBehaviour {
    GameObject? DirectionalLightClone { get; set; }

    IEnumerator EnableNightVision() {
        HDAdditionalLightData hdDef = null;
        while (true) {
            TimeOfDay timeOfDay = TimeOfDay.Instance;
            StartOfRound startOfRound = StartOfRound.Instance;
            if (startOfRound == null) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            if (timeOfDay == null) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            timeOfDay.insideLighting = false;

            if (timeOfDay.sunIndirect == null) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            if (hdDef == null) {
                hdDef = timeOfDay.sunIndirect.GetComponent<HDAdditionalLightData>();
            }

            if (hdDef != null) {
                hdDef.lightDimmer = float.MaxValue;
                hdDef.distance = float.MaxValue;
            }

            startOfRound.blackSkyVolume.weight = 0;
            yield return new WaitForEndOfFrame();
        }
    }


    void Start() {
        _ = this.StartCoroutine(this.EnableNightVision());
    }
}
