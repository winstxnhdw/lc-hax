using System.Collections;
using UnityEngine;

namespace Hax;

public sealed class InstantInteractMod : MonoBehaviour {
    InteractTrigger[] InteractTriggers => HaxObjects.Instance?.InteractTriggers.Objects ?? [];

    IEnumerator SetTimeToHold() {
        while (true) {
            this.InteractTriggers.ForEach(interactTrigger =>
                interactTrigger.timeToHold = interactTrigger.name is "EntranceTeleportB(Clone)" ? 0.3f : 0.0f
            );

            yield return new WaitForSeconds(5.0f);
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.SetTimeToHold());
    }
}
