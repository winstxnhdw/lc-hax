using System.Collections;
using UnityEngine;

namespace Hax;

public class NightVisionMod : MonoBehaviour {
    GameObject? DirectionalLightClone { get; set; }

    IEnumerator EnableNightVision() {
        while (true) {
            TimeOfDay timeOfDay = TimeOfDay.Instance;
            timeOfDay.sunDirect.enabled = true;
            timeOfDay.sunDirect.intensity = 1.0f;
            timeOfDay.sunIndirect.enabled = true;
            timeOfDay.sunIndirect.intensity = 1.0f;

            yield return new WaitForSeconds(1.0f);
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.EnableNightVision());
    }
}
