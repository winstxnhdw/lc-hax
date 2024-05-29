using System.Collections;
using UnityEngine;
using UnityObject = UnityEngine.Object;

internal class SingleObjectPool<T> where T : UnityObject
{
    internal SingleObjectPool(MonoBehaviour self, float renewInterval = 1.0f)
    {
        self.StartCoroutine(RenewObject(renewInterval));
    }

    internal T? Object { get; private set; }

    internal void Renew()
    {
        Object = UnityObject.FindAnyObjectByType<T>();
    }

    private IEnumerator RenewObject(float renewInterval)
    {
        WaitForSeconds waitForRenewInterval = new(renewInterval);

        while (true)
        {
            Renew();
            yield return waitForRenewInterval;
        }
    }
}