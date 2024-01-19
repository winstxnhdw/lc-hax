using System.Collections;
using UnityEngine;

public sealed class InstantInteractMod : MonoBehaviour {
    IEnumerator SetTimeToHold(object[] args) {
        WaitForSeconds waitForFiveSeconds = new(5.0f);

        while (true) {
            HaxObjects.Instance?.InteractTriggers?.WhereIsNotNull().ForEach(interactTrigger =>
                interactTrigger.timeToHold = interactTrigger.name is "EntranceTeleportB(Clone)" ? 0.3f : 0.0f
            );

            yield return waitForFiveSeconds;
        }
    }

    void Start() {
        _ = this.StartResilientCoroutine(this.SetTimeToHold);
    }
}
