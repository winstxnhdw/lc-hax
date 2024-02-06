using System.Collections;
using GameNetcodeStuff;
using UnityEngine;

public static class MouthDogController {

    public static void UsePrimarySkill(this MouthDogAI instance) {
        instance.SetState(MouthDog.Lunge);
    }

    public static void UseSecondarySkill(this MouthDogAI instance) {
        instance.EnterChaseMode();
    }

    public static void EnterChaseMode(this MouthDogAI instance) {
        _ = instance.StartCoroutine(instance.Reflect().InvokeInternalMethod<IEnumerator>("enterChaseMode"));
    }

    public static string GetPrimarySkillName(this MouthDogAI _) {
        return "Lunge";
    }

    public static void ChaseLocalPlayerInternal(this MouthDogAI instance) {
        _ = instance.Reflect().InvokeInternalMethod("ChaseLocalPlayer");
    }
    public static bool GetInLunge(this MouthDogAI instance) {
        return instance.Reflect().GetInternalField<bool>("inLunge");
    }
    public static void SetInLunge(this MouthDogAI instance, bool value) {
        _ = instance.Reflect().SetInternalField("inLunge", value);
    }
    public static void EnterLunge(this MouthDogAI instance) {
        _ = instance.Reflect().InvokeInternalMethod("EnterLunge");
    }
    public static void SetState(this MouthDogAI instance, MouthDog state) {
        if (instance.IsInState(state)) return;
        instance.SwitchToBehaviourServerRpc((int)state);
    }

    public static bool IsInState(this MouthDogAI instance, MouthDog state) {
        return instance.currentBehaviourStateIndex == (int)state;
    }

    public static void ChasePlayer(this MouthDogAI instance, PlayerControllerB player) {
        if (instance == null) return;
        if (player == null) return;
        if (instance.currentBehaviourStateIndex is 0 or 1) {
            instance.ChaseLocalPlayerInternal();
        }
        else {
            if (instance.currentBehaviourStateIndex != 2 || instance.GetInLunge())
                return;
            instance.transform.LookAt(player.transform.position);
            instance.transform.localEulerAngles = new Vector3(0.0f, player.transform.eulerAngles.y, 0.0f);
            instance.SetInLunge(true);
            instance.EnterLunge();
        }
    }

    public enum MouthDog {
        Roaming = 0,
        Suspicious = 1,
        ChaseMode = 2,
        Lunge = 3
    }

}

