using System.Collections;
using UnityEngine;
using UnityObject = UnityEngine.Object;

internal class MultiObjectPool<T> where T : UnityObject
{
    internal MultiObjectPool(MonoBehaviour self, float renewInterval = 1.0f)
    {
        self.StartCoroutine(RenewObjects(renewInterval));
    }

    internal T?[] Objects { get; private set; } = [];

    internal void Renew()
    {
        Objects = UnityObject.FindObjectsByType<T>(FindObjectsSortMode.None);
    }

    private IEnumerator RenewObjects(float renewInterval)
    {
        WaitForSeconds waitForRenewInterval = new(renewInterval);

        while (true)
        {
            Renew();
            yield return waitForRenewInterval;
        }
    }
}