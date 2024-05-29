using Hax;
using UnityEngine;

internal sealed class StunClickMod : MonoBehaviour
{
    private Collider[] Colliders { get; } = new Collider[100];
    private RaycastHit[] RaycastHits { get; } = new RaycastHit[100];

    private void OnEnable()
    {
        InputListener.OnLeftButtonPress += Stun;
    }

    private void OnDisable()
    {
        InputListener.OnLeftButtonPress -= Stun;
    }

    private bool IsHoldingADefensiveWeapon()
    {
        return Helper.LocalPlayer?.currentlyHeldObjectServer.itemProperties is { isDefensiveWeapon: true };
    }

    private void StunJam(Collider collider)
    {
        if (collider.TryGetComponent(out EnemyAICollisionDetect enemy)) enemy.mainScript.SetEnemyStunned(true, 5.0f);

        if (!collider.TryGetComponent(out Turret _) && !collider.TryGetComponent(out Landmine _) &&
            !collider.TryGetComponent(out SpikeRoofTrap _)) return;

        if (!collider.TryGetComponent(out TerminalAccessibleObject terminalAccessibleObject)) return;

        terminalAccessibleObject.CallFunctionFromTerminal();
    }

    private void Stun()
    {
        if (!Setting.EnableStunOnLeftClick) return;
        if (Helper.CurrentCamera is not Camera camera) return;
        if (IsHoldingADefensiveWeapon()) return;

        RaycastHits.SphereCastForward(camera.transform).Range().ForEach(i =>
        {
            var collider = RaycastHits[i].collider;
            StunJam(collider);
        });

        Physics.OverlapSphereNonAlloc(camera.transform.position, 5.0f, Colliders).Range().ForEach(i =>
        {
            var collider = Colliders[i];
            StunJam(collider);
        });
    }
}