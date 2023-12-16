using System;
using Unity.Netcode;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static ShipBuildModeManager? ShipBuildModeManager => ShipBuildModeManager.Instance;

    static NetworkObject GetNetworkObject<M>(M gameObject) where M : MonoBehaviour =>
        gameObject.GetComponentInChildren<PlaceableShipObject>()
                  .parentObject
                  .GetComponent<NetworkObject>();

    public static Action<float> PlaceObjectAtTransform<T, M>(
        T targetObject,
        M gameObject,
        Vector3 positionOffset = new(),
        Vector3 rotationOffset = new()
    ) where T : Transform where M : MonoBehaviour => (_) => {
        NetworkObject networkObject = Helper.GetNetworkObject(gameObject);
        Helper.ShipBuildModeManager?.PlaceShipObjectServerRpc(
            targetObject.position + positionOffset,
            targetObject.eulerAngles + rotationOffset,
            networkObject,
            -1
        );
    };

    public static Action<float> PlaceObjectAtTransform<T, M>(
        ObjectPlacement<T, M> placement
    ) where T : Transform where M : MonoBehaviour => Helper.PlaceObjectAtTransform(
        placement.TargetObject,
        placement.GameObject,
        placement.PositionOffset,
        placement.RotationOffset
    );

    public static Action<float> PlaceObjectAtPosition<T, M>(
        T targetObject,
        M gameObject,
        Vector3 positionOffset = new(),
        Vector3 rotationOffset = new()
    ) where T : Transform where M : MonoBehaviour => (_) => {
        NetworkObject networkObject = Helper.GetNetworkObject(gameObject);
        Helper.ShipBuildModeManager?.PlaceShipObjectServerRpc(
            targetObject.position + positionOffset,
            rotationOffset,
            networkObject,
            -1
        );
    };

    public static Action<float> PlaceObjectAtPosition<M>(
        M gameObject,
        Vector3 position,
        Vector3 rotation
    ) where M : MonoBehaviour => (_) => {
        NetworkObject networkObject = Helper.GetNetworkObject(gameObject);
        Helper.ShipBuildModeManager?.PlaceShipObjectServerRpc(
            position,
            rotation,
            networkObject,
            -1
        );
    };

    public static Action<float> PlaceObjectAtPosition<T, M>(
        ObjectPlacement<T, M> placement
    ) where T : Transform where M : MonoBehaviour => Helper.PlaceObjectAtPosition(
        placement.TargetObject,
        placement.GameObject,
        placement.PositionOffset,
        placement.RotationOffset
    );
}
