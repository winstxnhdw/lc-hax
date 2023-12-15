using System;
using System.Collections;
using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Hax;

public class ClearVisionMod : MonoBehaviour {
    private Transform? cloneSunIndirect = null;

    IEnumerator SetClearVision() {
        while (true) {
            if (!Helper.StartOfRound.IsNotNull(out StartOfRound startOfRound)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            if (!TimeOfDay.Instance.IsNotNull(out TimeOfDay timeOfDay)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            if (!timeOfDay.sunIndirect.IsNotNull(out Light sunIndirect)) {
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

            HDAdditionalLightData? lightData = null;
            try {
                lightData = sunIndirect.GetComponent<HDAdditionalLightData?>();

            }
            catch { }
            if (lightData == null) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            sunAnimator.enabled = false;
            sunIndirect.transform.eulerAngles = new Vector3(90, 0, 0);
            sunDirect.transform.eulerAngles = new Vector3(90, 0, 0);
            lightData.lightDimmer = float.MaxValue;
            lightData.distance = float.MaxValue;
            timeOfDay.insideLighting = false;
            startOfRound.blackSkyVolume.weight = 0;

            if (!this.cloneSunIndirect.IsNotNull(out Transform _)) {

                this.cloneSunIndirect = Instantiate(sunIndirect).transform;
            }

            if (Helper.CurrentCamera.IsNotNull(out Camera cam)
                && this.cloneSunIndirect.IsNotNull(out Transform cloneSunIndirect)) {
                cloneSunIndirect.SetParent(cam.transform);
                cloneSunIndirect.localPosition = Vector3.zero;
            }

            yield return new WaitForEndOfFrame();
        }
    }
    void Start() {
        _ = this.StartCoroutine(this.SetClearVision());
    }
}
