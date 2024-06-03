#region

using Unity.Netcode;
using UnityEngine;

#endregion

namespace Hax;

static partial class Helper {
    internal static ShipBuildModeManager? ShipBuildModeManager => ShipBuildModeManager.Instance;

    static NetworkObject GetNetworkObject<M>(M gameObject) where M : MonoBehaviour =>
        gameObject.GetComponentInChildren<PlaceableShipObject>()
            .parentObject
            .GetComponent<NetworkObject>();

    internal static void PlaceObjectAtTransform<T, M>(
        T targetObject,
        M gameObject,
        Vector3 positionOffset = new(),
        Vector3 rotationOffset = new()
    ) where T : Transform where M : MonoBehaviour {
        NetworkObject networkObject = GetNetworkObject(gameObject);
        ShipBuildModeManager?.PlaceShipObjectServerRpc(
            targetObject.position + positionOffset,
            targetObject.eulerAngles + rotationOffset,
            networkObject,
            -1
        );
    }

    internal static void PlaceObjectAtTransform<T, M>(
        ObjectPlacement<T, M> placement
    ) where T : Transform where M : MonoBehaviour =>
        PlaceObjectAtTransform(
            placement.TargetObject,
            placement.GameObject,
            placement.PositionOffset,
            placement.RotationOffset
        );

    internal static void PlaceObjectAtPosition<T, M>(
        T targetObject,
        M gameObject,
        Vector3 positionOffset = new(),
        Vector3 rotationOffset = new()
    ) where T : Transform where M : MonoBehaviour {
        NetworkObject networkObject = GetNetworkObject(gameObject);
        ShipBuildModeManager?.PlaceShipObjectServerRpc(
            targetObject.position + positionOffset,
            rotationOffset,
            networkObject,
            -1
        );
    }

    internal static void PlaceObjectAtPosition<M>(
        M gameObject,
        Vector3 position,
        Vector3 rotation = new()
    ) where M : MonoBehaviour {
        NetworkObject networkObject = GetNetworkObject(gameObject);
        ShipBuildModeManager?.PlaceShipObjectServerRpc(
            position,
            rotation,
            networkObject,
            -1
        );
    }

    internal static void PlaceObjectAtPosition<T, M>(
        ObjectPlacement<T, M> placement
    ) where T : Transform where M : MonoBehaviour =>
        PlaceObjectAtPosition(
            placement.TargetObject,
            placement.GameObject,
            placement.PositionOffset,
            placement.RotationOffset
        );
}
