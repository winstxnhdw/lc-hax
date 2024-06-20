using System;
using System.Collections.Generic;
using System.Text;
using GameNetcodeStuff;
using UnityEngine;


class CustomPlayerTrigger : MonoBehaviour {

    internal Action <PlayerControllerB> onPlayerEnter;
    internal Action <PlayerControllerB> onPlayerExit;

    void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag("Player")) {
            // get the player controller
            PlayerControllerB player = other.gameObject.GetComponent<PlayerControllerB>();
            if (player) {
                onPlayerEnter?.Invoke(player);
            }

        }

    }

    void OnTriggerExit(Collider other) {

        if (other.gameObject.CompareTag("Player")) {
            PlayerControllerB player = other.gameObject.GetComponent<PlayerControllerB>();
            if (player) {
                this.onPlayerExit?.Invoke(player);
            }

        }
    }
}

