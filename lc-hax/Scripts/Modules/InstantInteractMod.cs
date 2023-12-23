using System.Linq;
using System.Collections;
using UnityEngine;

namespace Hax;

public sealed class InstantInteractMod : MonoBehaviour {
    IEnumerator SetTimeToHold() {
        while (true) {
            HaxObjects.Instance?.InteractTriggers.Objects.ToList().ForEach(interactTrigger => {
                interactTrigger.timeToHold = 0.0f;
                interactTrigger.cooldownTime = 0.0f;
                interactTrigger.currentCooldownValue = 0.0f;
            });

            yield return new WaitForSeconds(5.0f);
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.SetTimeToHold());
    }
}
