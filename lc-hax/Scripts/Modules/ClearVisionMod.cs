using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Hax;

public class ClearVisionMod : MonoBehaviour {
    IEnumerator SetClearVision() {
        float timer = 0;
        while (true) {
            timer += Time.deltaTime;

            if (Helper.StartOfRound.IsNotNull(out StartOfRound startOfRound)
                && startOfRound.blackSkyVolume.IsNotNull(out UnityEngine.Rendering.Volume blackSkyVolume)) {
                blackSkyVolume.weight = 0;
            }

            if (TimeOfDay.Instance.IsNotNull(out TimeOfDay timeOfDay)) {
                timeOfDay.insideLighting = false;
            }

            if (timeOfDay.sunAnimator.IsNotNull(out Animator sunAnimator)) {
                sunAnimator.enabled = false;
            }

            if (timeOfDay.sunIndirect.IsNotNull(out Light sunIndirect)) {
                sunIndirect.transform.eulerAngles = new Vector3(90, 0, 0);
            }

            if (sunIndirect.GetComponent<HDAdditionalLightData>().IsNotNull(out HDAdditionalLightData lightData)) {
                lightData.lightDimmer = float.MaxValue;
                lightData.distance = float.MaxValue;
            }

            if (timeOfDay.sunDirect.IsNotNull(out Light sunDirect)) {
                sunDirect.transform.eulerAngles = new Vector3(90, 0, 0);
            }


            try {
                Console.Print("t", $"test{timer}");
            }
            catch { }

            yield return new WaitForEndOfFrame();
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.SetClearVision());
    }
}
