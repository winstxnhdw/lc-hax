using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Hax;

public sealed class ClearVisionMod : MonoBehaviour {
    IEnumerator SetNightVision() {
        WaitForEndOfFrame waitForEndOfFrame = new();

        while (true) {
            if (Helper.StartOfRound is not StartOfRound startOfRound) {
                yield return waitForEndOfFrame;
                continue;
            }

            if (Helper.CurrentCamera is not Camera camera) {
                yield return waitForEndOfFrame;
                continue;
            }

            if (TimeOfDay.Instance.Unfake() is not TimeOfDay timeOfDay) {
                yield return waitForEndOfFrame;
                continue;
            }

            if (timeOfDay.sunAnimator.Unfake() is not Animator sunAnimator) {
                yield return waitForEndOfFrame;
                continue;
            }

            if (timeOfDay.sunDirect.Unfake() is not Light sunDirect) {
                yield return waitForEndOfFrame;
                continue;
            }

            if (timeOfDay.sunIndirect.Unfake() is not Light sunIndirect) {
                yield return waitForEndOfFrame;
                continue;
            }

            if (sunIndirect.TryGetComponent(out HDAdditionalLightData lightData)) {
                yield return waitForEndOfFrame;
                continue;
            }

            sunAnimator.enabled = false;
            sunIndirect.transform.eulerAngles = new Vector3(90, 0, 0);
            sunIndirect.transform.position = camera.transform.position;
            sunIndirect.color = Color.white;
            sunIndirect.intensity = 5;
            sunIndirect.enabled = true;
            sunDirect.transform.eulerAngles = new Vector3(90, 0, 0);
            sunDirect.enabled = true;
            lightData.lightDimmer = float.MaxValue;
            lightData.distance = float.MaxValue;
            timeOfDay.insideLighting = false;
            startOfRound.blackSkyVolume.weight = 0;

            yield return waitForEndOfFrame;
        }
    }

    IEnumerator DisableVisor(object[] args) {
        WaitForSeconds waitForTenSeconds = new(10.0f);

        while (true) {
            Helper.LocalPlayer?.localVisor.gameObject.SetActive(false);
            yield return waitForTenSeconds;
        }
    }

    IEnumerator DisableFog(object[] args) {
        WaitForSeconds waitForFiveSeconds = new(5.0f);

        while (true) {
            HaxObjects
                .Instance?
                .LocalVolumetricFogs
                .ForEach(localVolumetricFog =>
                    localVolumetricFog?.gameObject.SetActive(false)
                );

            yield return waitForFiveSeconds;
        }
    }

    IEnumerator DisableSteamValves(object[] args) {
        WaitForSeconds waitForFiveSeconds = new(5.0f);

        while (true) {
            HaxObjects
                .Instance?
                .SteamValves
                .ForEach(valve =>
                    valve?.valveSteamParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear)
                );

            yield return waitForFiveSeconds;
        }
    }

    void Start() {
        _ = this.StartResilientCoroutine(this.DisableFog);
        _ = this.StartResilientCoroutine(this.DisableSteamValves);
        _ = this.StartResilientCoroutine(this.DisableVisor);
        _ = this.StartCoroutine(this.SetNightVision());
    }
}
