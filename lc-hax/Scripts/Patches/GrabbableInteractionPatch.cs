#region

using HarmonyLib;
using UnityEngine;
using Random = System.Random;

#endregion

[HarmonyPatch(typeof(GrabbableObject), "ItemActivate")]
public static class GrabbableInteractionPatch {
    public static void Postfix(GrabbableObject __instance, bool used, ref bool buttonDown) {
        if (__instance != null)
            if (buttonDown) {
                // toggle item noises and animations
                if (__instance is AnimatedItem animation) {
                    if (animation.itemAnimator != null) {
                        if (animation.itemAudio.isPlaying)
                            StopAnimatedNoise(animation);
                        else
                            StartAnimatedNoise(animation);
                    }
                    else
                        PlayStaticPropNoise(animation);

                    return;
                }

                if (__instance is WhoopieCushionItem cushion) {
                    cushion.Fart();
                    return;
                }
            }
    }

    internal static void PlayStaticPropNoise(AnimatedItem item) {
        if (item == null) return;
        if (item.itemAudio.isPlaying)
            item.itemAudio.Stop();
        else {
            if (item.grabAudio != null) {
                item.itemAudio.clip = item.grabAudio;
                item.itemAudio.loop = item.loopGrabAudio;
            }
            else if (item.dropAudio != null) {
                item.itemAudio.clip = item.dropAudio;
                item.itemAudio.loop = item.loopDropAudio;
            }

            item.itemAudio.Play();
        }
    }

    internal static void StopAnimatedNoise(AnimatedItem item) {
        if (item == null) return;
        Reflector<AnimatedItem> reflector = item.Reflect();
        Random? itemRandomChance = reflector.GetInternalField<Random>("itemRandomChance");

        if (item.itemAnimator != null) item.itemAnimator.SetBool(item.grabItemBoolString, false);

        if (item.chanceToTriggerAlternateMesh > 0)
            item.gameObject.GetComponent<MeshFilter>().mesh = reflector.GetInternalField<Mesh>("normalMesh");

        if (!item.makeAnimationWhenDropping) {
            item.itemAudio.Stop();
            return;
        }

        if (itemRandomChance.Next(0, 100) < item.chanceToTriggerAnimation) {
            item.itemAudio.Stop();
            return;
        }

        if (item.itemAnimator != null) item.itemAnimator.SetTrigger(item.dropItemTriggerString);

        if (item.itemAudio != null) {
            item.itemAudio.loop = item.loopDropAudio;
            item.itemAudio.clip = item.dropAudio;
            item.itemAudio.Play();
            if (item.itemAudioLowPassFilter != null) item.itemAudioLowPassFilter.cutoffFrequency = 20000f;

            item.itemAudio.volume = 1f;
        }
    }

    internal static void StartAnimatedNoise(AnimatedItem item) {
        if (item == null) return;
        Reflector<AnimatedItem> reflector = item.Reflect();
        Random? itemRandomChance = reflector.GetInternalField<Random>("itemRandomChance");
        if (item.itemAudioLowPassFilter != null) item.itemAudioLowPassFilter.cutoffFrequency = 20000f;
        item.itemAudio.volume = 1f;
        if (item.chanceToTriggerAlternateMesh > 0) {
            if (itemRandomChance.Next(0, 100) < item.chanceToTriggerAlternateMesh) {
                item.gameObject.GetComponent<MeshFilter>().mesh = item.alternateMesh;
                item.itemAudio.Stop();
                return;
            }

            item.gameObject.GetComponent<MeshFilter>().mesh = reflector.GetInternalField<Mesh>("normalMesh");
        }

        if (!reflector.GetInternalField<bool>("wasInPocket")) {
            if (itemRandomChance.Next(0, 100) > item.chanceToTriggerAnimation) {
                item.itemAudio.Stop();
                return;
            }
        }
        else
            reflector.SetInternalField("wasInPocket", false);

        if (item.itemAnimator != null) item.itemAnimator.SetBool(item.grabItemBoolString, true);
        if (item.itemAudio != null) {
            item.itemAudio.clip = item.grabAudio;
            item.itemAudio.loop = item.loopGrabAudio;
            item.itemAudio.Play();
        }
    }
}
