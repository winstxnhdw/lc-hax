using System.Collections;
using UnityEngine;

internal sealed class InstantInteractMod : MonoBehaviour
{
    private IEnumerator SetTimeToHold(object[] args)
    {
        WaitForSeconds waitForFiveSeconds = new(5.0f);

        while (true)
        {
            HaxObjects.Instance?.InteractTriggers?.WhereIsNotNull().ForEach(interactTrigger =>
            {
                interactTrigger.interactable = true;
                interactTrigger.timeToHold = interactTrigger.name is "EntranceTeleportB(Clone)" ? 0.3f : 0.0f;
            });

            yield return waitForFiveSeconds;
        }
    }

    private void Start()
    {
        this.StartResilientCoroutine(SetTimeToHold);
    }
}