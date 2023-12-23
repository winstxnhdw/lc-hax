using System.Collections;
using UnityEngine;

namespace Hax;

public sealed class InstantInteractMod : MonoBehaviour {
    IEnumerator SetTimeToHold() {
        while (true) {
            HaxObjects.Instance?.InteractTriggers.Objects?.ForEach(interactTrigger =>
                interactTrigger.timeToHold = interactTrigger.transform.parent.name is "EntranceTeleportB" ? 0.3f : 0.0f
            );

            yield return new WaitForSeconds(5.0f);
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.SetTimeToHold());
    }
}
