using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Hax;

public class ClearVisionMod : MonoBehaviour {
    IEnumerator SetNightVision() {
        while (true) {
            if (!Helper.StartOfRound.IsNotNull(out StartOfRound startOfRound)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            if (!Helper.CurrentCamera.IsNotNull(out Camera cam)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            if (!TimeOfDay.Instance.IsNotNull(out TimeOfDay timeOfDay)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            if (!timeOfDay.sunAnimator.IsNotNull(out Animator sunAnimator)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            if (!timeOfDay.sunDirect.IsNotNull(out Light sunDirect)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            if (!timeOfDay.sunIndirect.IsNotNull(out Light sunIndirect)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            if (!Helper.Try(sunIndirect.GetComponent<HDAdditionalLightData>).IsNotNull(out HDAdditionalLightData lightData)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            sunAnimator.enabled = false;
            sunIndirect.transform.eulerAngles = new Vector3(90, 0, 0);
            sunIndirect.transform.position = cam.transform.position;
            sunIndirect.color = Color.white;
            sunIndirect.intensity = 10;
            sunIndirect.enabled = true;
            sunDirect.transform.eulerAngles = new Vector3(90, 0, 0);
            sunDirect.enabled = true;
            lightData.lightDimmer = float.MaxValue;
            lightData.distance = float.MaxValue;
            timeOfDay.insideLighting = false;
            startOfRound.blackSkyVolume.weight = 0;

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DisableFog() {
        while (true) {
            HaxObject.Instance?
                     .ToggleFogTriggers
                     .Objects
                     .ToList()
                     .ForEach(toggleFogTrigger => {
                         toggleFogTrigger.fog1.enabled = false;
                         toggleFogTrigger.fog2.enabled = false;
                     });

            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator DisableSteamValves() {
        while (true) {
            HaxObject.Instance?
                     .SteamValves
                     .Objects
                     .ToList()
                     .ForEach(valve => valve.valveSteamParticle
                     .Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear));

            yield return new WaitForSeconds(5.0f);
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.DisableFog());
        _ = this.StartCoroutine(this.SetNightVision());
        _ = this.StartCoroutine(this.DisableSteamValves());
    }
}
