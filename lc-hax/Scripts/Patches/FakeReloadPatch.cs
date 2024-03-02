using GameNetcodeStuff;
using HarmonyLib;

class FakeReloadPatch {

    static bool InterruptDestroyItem { get; set; }

    [HarmonyPatch(typeof(ShotgunItem))]
    class ShotGun {
        [HarmonyPatch("FindAmmoInInventory"), HarmonyPrefix] // This makes the shotgun look for a item slot that has a Shotgun Shell. Patch makes FindAmmoInInventory always result to 0 so it's not null.
        static bool PrefixFindAmmoInInventory(ShotgunItem __instance, ref int __result) {
            __result = -2;
            return false;
        }

        [HarmonyPatch("ItemInteractLeftRight"), HarmonyPrefix]
        static bool PrefixItemInteractLeftRight(ShotgunItem __instance, ref int ___shellsLoaded) { // This checks if the shotgun is already loaded. Patch makes it where if it's full it unloads it so you can reload again.
            if (___shellsLoaded >= 2) {
                ___shellsLoaded = 0;
                return false;
            }
            return true;
        }

        [HarmonyPatch("reloadGunAnimation"), HarmonyPrefix] // Sets InterruptDestroyItem to true if Reload Starts.
        static void PrefixReloadGunAnimation() => FakeReloadPatch.InterruptDestroyItem = true;
        [HarmonyPatch("StopUsingGun"), HarmonyPrefix] // Sets InterruptDestroyItem to false if Shotgun is Dropped or Pocketed.
        static void PrefixStopUsingGun() => FakeReloadPatch.InterruptDestroyItem = false;
    }

    [HarmonyPatch(typeof(PlayerControllerB))]
    class DestroyItem {
        [HarmonyPatch("DestroyItemInSlotAndSync"), HarmonyPrefix] // if InterruptDestroyItem it's true it immediately returns DestroyItemInSlotAndSync to prevent ShotgunItem.reloadGunAnimation to call it.
        static bool Prefix() => !FakeReloadPatch.InterruptDestroyItem;
    }
}
