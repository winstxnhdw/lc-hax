using UnityEngine;

namespace Hax;

public class ScanMod : MonoBehaviour {
    void Update() {
        ScanNodeProperties? scanNodeProperties = HaxObjects.Instance?.ScanNodeProperties.Object;

        if (scanNodeProperties == null) {
            return;
        }

        scanNodeProperties.requiresLineOfSight = false;
        scanNodeProperties.maxRange = 10000;
        scanNodeProperties.minRange = 1;
    }
}
