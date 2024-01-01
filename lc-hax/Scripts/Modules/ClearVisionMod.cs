using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Hax;

public sealed class ClearVisionMod : MonoBehaviour {
    IEnumerator SetNightVision() {
        while (true) {
            if (!Helper.StartOfRound.IsNotNull(out StartOfRound startOfRound)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) {
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

            if (!sunIndirect.TryGetComponent(out HDAdditionalLightData lightData)) {
                yield return new WaitForEndOfFrame();
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
            startOfRound.localPlayerController.localVisor.gameObject.SetActive(false);

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DisableFog() {
        while (true) {
            HaxObjects
                .Instance?
                .LocalVolumetricFogs
                .ForEach(localVolumetricFog =>
                    localVolumetricFog?.gameObject.SetActive(false)
                );

            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator DisableSteamValves() {
        while (true) {
            HaxObjects
                .Instance?
                .SteamValves
                .ForEach(valve =>
                    valve?.valveSteamParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear)
                );

            yield return new WaitForSeconds(5.0f);
        }
    }

    void Start() {
        _ = this.StartResilientCoroutine(this.DisableFog);
        _ = this.StartResilientCoroutine(this.DisableSteamValves);
        _ = this.StartCoroutine(this.SetNightVision());
    }
}
