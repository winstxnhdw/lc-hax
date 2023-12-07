using System;
using Unity.Netcode;
using UnityEngine;

namespace Hax;

public static partial class Helpers {
    public static Action<float> PlaceObjectAtTransform<T, M>(
        T targetObject,
        M gameObject,
        Vector3 positionOffset = new(),
        Vector3 rotationOffset = new()
    ) where T : Transform where M : MonoBehaviour {
        return (_) => {
            NetworkObject networkObject =
                gameObject.GetComponentInChildren<PlaceableShipObject>()
                          .parentObject
                          .GetComponent<NetworkObject>();

            ShipBuildModeManager.Instance.PlaceShipObjectServerRpc(
                targetObject.position + positionOffset,
                targetObject.eulerAngles + rotationOffset,
                networkObject,
                -1
            );
        };
    }

    public static Action<float> PlaceObjectAtPosition<T, M>(
        T targetObject,
        M gameObject,
        Vector3 positionOffset = new(),
        Vector3 rotationOffset = new()
    ) where T : Transform where M : MonoBehaviour {
        return (_) => {
            NetworkObject networkObject =
                gameObject.GetComponentInChildren<PlaceableShipObject>()
                          .parentObject
                          .GetComponent<NetworkObject>();

            ShipBuildModeManager.Instance.PlaceShipObjectServerRpc(
                targetObject.position + positionOffset,
                rotationOffset,
                networkObject,
                -1
            );
        };
    }
}
