#pragma warning disable IDE1006

using System.Collections;
using UnityEngine;
using HarmonyLib;

[HarmonyPatch(typeof(ShotgunItem))]
class FakeReloadPatch {
    static IEnumerator CustomReloadCoroutine(ShotgunItem shotgun) {
        yield return new WaitForSeconds(0.3f);

        shotgun.gunAudio.PlayOneShot(shotgun.gunReloadSFX);
        shotgun.gunAnimator.SetBool("Reloading", true);
        shotgun.ReloadGunEffectsServerRpc(true);

        yield return new WaitForSeconds(0.95f);

        shotgun.shotgunShellInHand.enabled = true;
        shotgun.shotgunShellInHandTransform.SetParent(shotgun.playerHeldBy.leftHandItemTarget);
        shotgun.shotgunShellInHandTransform.localPosition = new Vector3(-0.0555f, 0.1469f, -0.0655f);
        shotgun.shotgunShellInHandTransform.localEulerAngles = new Vector3(-1.956f, 143.856f, -16.427f);

        yield return new WaitForSeconds(0.95f);

        shotgun.shellsLoaded = Mathf.Clamp(shotgun.shellsLoaded + 1, 0, 2);
        shotgun.shotgunShellLeft.enabled = true;

        if (shotgun.shellsLoaded == 2) {
            shotgun.shotgunShellRight.enabled = true;
        }

        shotgun.shotgunShellInHand.enabled = false;
        shotgun.shotgunShellInHandTransform.SetParent(shotgun.transform);

        yield return new WaitForSeconds(0.45f);

        shotgun.gunAudio.PlayOneShot(shotgun.gunReloadFinishSFX);
        shotgun.gunAnimator.SetBool("Reloading", false);
        shotgun.playerHeldBy.playerBodyAnimator.SetBool("ReloadShotgun", false);
        shotgun.playerHeldBy.playerBodyAnimator.SetBool("ReloadShotgun2", false);
        shotgun.isReloading = false;
        shotgun.ReloadGunEffectsServerRpc(false);
    }

    /// <summary>
    /// Allow reloads even if the shotgun is full
    /// </summary>
    [HarmonyPatch(nameof(ShotgunItem.ItemInteractLeftRight))]
    static void Prefix(ShotgunItem __instance) => __instance.shellsLoaded = 1;

    /// <summary>
    /// Allow reloads without any shells in the inventory
    /// </summary>
    [HarmonyPatch("ReloadedGun")]
    static bool Prefix(ref bool __result) {
        __result = true;
        return false;
    }

    /// <summary>
    /// Do not attempt to destroy any shells
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch("reloadGunAnimation")]
    static bool ReloadGunAnimationPrefix(ShotgunItem __instance) {
        if (__instance.shellsLoaded <= 0) {
            __instance.playerHeldBy.playerBodyAnimator.SetBool("ReloadShotgun", true);
            __instance.shotgunShellLeft.enabled = false;
        }

        else {
            __instance.playerHeldBy.playerBodyAnimator.SetBool("ReloadShotgun2", true);
        }

        __instance.isReloading = true;
        __instance.shotgunShellRight.enabled = false;
        _ = __instance.StartCoroutine(FakeReloadPatch.CustomReloadCoroutine(__instance));

        return false;
    }
}
