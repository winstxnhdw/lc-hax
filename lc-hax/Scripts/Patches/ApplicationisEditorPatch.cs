using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(Application), "isEditor", MethodType.Getter)]
public static class ApplicationIsEditorPatch {
    private static bool Prefix(ref bool __result) {
        __result = Setting.isEditorMode;
        return false;
    }
}
