#region

using System.Collections;
using UnityEngine;

#endregion

sealed class InstantInteractMod : MonoBehaviour {
    IEnumerator SetTimeToHold(object[] args) {
        WaitForSeconds waitForFiveSeconds = new(5.0f);

        while (true) {
            HaxObjects.Instance?.InteractTriggers?.WhereIsNotNull().ForEach(interactTrigger => {
                interactTrigger.interactable = true;
                interactTrigger.timeToHold = interactTrigger.name is "EntranceTeleportB(Clone)" ? 0.3f : 0.0f;
            });

            yield return waitForFiveSeconds;
        }
    }

    void Start() => this.StartResilientCoroutine(this.SetTimeToHold);
}
