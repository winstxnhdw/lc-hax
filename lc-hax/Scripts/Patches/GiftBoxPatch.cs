#pragma warning disable IDE1006

using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

[HarmonyPatch(typeof(GiftBoxItem), nameof(GiftBoxItem.ItemActivate))]
class GiftBoxPatch {
    static bool Prefix(ref GiftBoxItem __instance) {
        //__instance.OpenGiftBoxServerRpc();
        return false;
    }

    private void PatchedOpenGift(GiftBoxItem item) {
        if(item == null) return;
        NetworkManager networkManager = base.NetworkManager;
        if (networkManager == null || !networkManager.IsListening) {
            return;
        }
        if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Server && (networkManager.IsClient || networkManager.IsHost)) {
            ServerRpcParams serverRpcParams;
            FastBufferWriter fastBufferWriter = base.__beginSendServerRpc(2878544999U, serverRpcParams, RpcDelivery.Reliable);
            base.__endSendServerRpc(ref fastBufferWriter, 2878544999U, serverRpcParams, RpcDelivery.Reliable);
        }
        if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Server || (!networkManager.IsServer && !networkManager.IsHost)) {
            return;
        }
        GameObject gameObject = null;
        int num = 0;
        Vector3 vector = Vector3.zero;
        if (this.objectInPresent == null) {
            Debug.LogError("Error: There is no object in gift box!");
        }
        else {
            Transform parent;
            if (((this.playerHeldBy != null && this.playerHeldBy.isInElevator) || StartOfRound.Instance.inShipPhase) && RoundManager.Instance.spawnedScrapContainer != null) {
                parent = RoundManager.Instance.spawnedScrapContainer;
            }
            else {
                parent = StartOfRound.Instance.elevatorTransform;
            }
            vector = base.transform.position + Vector3.up * 0.25f;
            gameObject = Object.Instantiate<GameObject>(this.objectInPresent, vector, Quaternion.identity, parent);
            GrabbableObject component = gameObject.GetComponent<GrabbableObject>();
            component.startFallingPosition = vector;
            base.StartCoroutine(this.SetObjectToHitGroundSFX(component));
            component.targetFloorPosition = component.GetItemFloorPosition(base.transform.position);
            if (this.previousPlayerHeldBy != null && this.previousPlayerHeldBy.isInHangarShipRoom) {
                this.previousPlayerHeldBy.SetItemInElevator(true, true, component);
            }
            num = this.objectInPresentValue;
            component.SetScrapValue(num);
            component.NetworkObject.Spawn(false);
        }
        if (gameObject != null) {
            this.OpenGiftBoxClientRpc(gameObject.GetComponent<NetworkObject>(), num, vector);
        }
        this.OpenGiftBoxNoPresentClientRpc();
    }
}
