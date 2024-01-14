// #pragma warning disable IDE0060
// using GameNetcodeStuff;
// using HarmonyLib;
// using UnityEngine;
// using Hax;

// [HarmonyPatch(typeof(PlayerControllerB), "KillPlayerClientRpc")]
// class FakeDeathPatch {
//     static void Prefix(ref int playerId, ref bool spawnBody, ref Vector3 bodyVelocity, ref int causeOfDeath, ref int deathAnimation) {
//         if (playerId == (int)Helper.LocalPlayer!.playerClientId) {
//             playerId = -1;
//         }
//     }
// }
