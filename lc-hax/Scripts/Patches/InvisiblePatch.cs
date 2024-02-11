#pragma warning disable IDE1006

using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB))]
class InvisiblePatch {
    static Vector3 LastNewPos { get; set; }
    static bool LastInElevator { get; set; }
    static bool LastInShipRoom { get; set; }
    static bool LastExhausted { get; set; }
    static bool LastIsPlayerGrounded { get; set; }

    [HarmonyPatch("UpdatePlayerPositionServerRpc")]
    static void Prefix(
        ref Vector3 newPos,
        ref bool inElevator,
        ref bool inShipRoom,
        ref bool exhausted,
        ref bool isPlayerGrounded
    ) {
        if (!Setting.EnableInvisible) return;

        InvisiblePatch.LastNewPos = newPos;
        InvisiblePatch.LastInElevator = inElevator;
        InvisiblePatch.LastInShipRoom = inShipRoom;
        InvisiblePatch.LastExhausted = exhausted;
        InvisiblePatch.LastIsPlayerGrounded = isPlayerGrounded;

        newPos = new Vector3(0.0f, -100.0f, 0.0f);
        inElevator = false;
        inShipRoom = false;
        exhausted = false;
        isPlayerGrounded = true;
    }

    [HarmonyPatch("UpdatePlayerPositionClientRpc")]
    static void Prefix(
        PlayerControllerB __instance,
        ref Vector3 newPos,
        ref bool inElevator,
        ref bool isInShip,
        ref bool exhausted,
        ref bool isPlayerGrounded
    ) {
        if (!Setting.EnableInvisible || !__instance.IsSelf()) return;

        newPos = InvisiblePatch.LastNewPos;
        inElevator = InvisiblePatch.LastInElevator;
        isInShip = InvisiblePatch.LastInShipRoom;
        exhausted = InvisiblePatch.LastExhausted;
        isPlayerGrounded = InvisiblePatch.LastIsPlayerGrounded;
    }
}
