using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class FollowerAnotherPlayerMod : MonoBehaviour {

    bool follow = false;
    Queue<Vector3> oneSecondOfPos = new Queue<Vector3>();

    void Start() {
    }
}
