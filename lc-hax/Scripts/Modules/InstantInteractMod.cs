using System.Linq;
using System.Collections;
using UnityEngine;

namespace Hax;

public sealed class InstantInteractMod : MonoBehaviour {
    IEnumerator SetTimeToHold() {
        while (true) {
            HaxObjects.Instance?.InteractTriggers.Objects.ToList().ForEach(interactTrigger => {
                interactTrigger.timeToHold = 0.0f;
            });

            FindObjectsOfType<EntranceTeleport>().ToList().ForEach(entranceTeleport => {
                if (entranceTeleport.name is not "EntranceTeleportB") return;
                entranceTeleport.GetComponent<InteractTrigger>().timeToHold = 0.3f;
            });

            yield return new WaitForSeconds(5.0f);
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.SetTimeToHold());
    }
}
