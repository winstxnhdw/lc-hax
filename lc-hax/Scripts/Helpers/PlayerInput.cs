#pragma warning disable CS8602

#region

using UnityEngine;

#endregion

namespace Hax;

static partial class Helper {
    internal static bool PlayerInput_Sprint() =>
        IngamePlayerSettings.playerInput.actions.FindAction("Sprint", false).ReadValue<float>() > 0f;

    internal static Vector2 PlayerInput_Move() =>
        IngamePlayerSettings.playerInput.actions.FindAction("Move", false).ReadValue<Vector2>();

    internal static bool PlayerInput_isMoving() => PlayerInput_Move().sqrMagnitude > 0.01f;
}
