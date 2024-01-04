/*using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB), "UpdatePlayerPositionServerRpc")]
class InvisiblePatch {
    static void Prefix(ref Vector3 pos, ref bool inElevator, ref bool exhausted, ref bool isPlayerGrounded) {
        if (Setting.EnableInvisible) {
            pos = new Vector3(0, -100, 0);
            inElevator = false;
            exhausted = false;
            isPlayerGrounded = true;
        }
    }
}
*/