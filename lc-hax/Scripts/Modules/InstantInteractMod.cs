using System.Collections;
using UnityEngine;

namespace Hax;

public sealed class InstantInteractMod : MonoBehaviour {
    string? GetInteractTriggerName(InteractTrigger? interactTrigger) => interactTrigger?.name;

    IEnumerator SetTimeToHold() {
        while (true) {
            HaxObjects.Instance?.InteractTriggers.Objects?.ForEach(interactTrigger => {
                if (interactTrigger is null) return;
                interactTrigger.timeToHold = interactTrigger.name is "EntranceTeleportB(Clone)" ? 0.3f : 0.0f;
            });

            yield return new WaitForSeconds(5.0f);
        }
    }

    void Start() {
        _ = this.StartResilientCoroutine(this.SetTimeToHold());
    }
}
