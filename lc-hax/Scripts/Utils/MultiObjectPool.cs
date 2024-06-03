#region

using System.Collections;
using UnityEngine;
using UnityObject = UnityEngine.Object;

#endregion

class MultiObjectPool<T> where T : UnityObject {
    internal MultiObjectPool(MonoBehaviour self, float renewInterval = 1.0f) =>
        self.StartCoroutine(this.RenewObjects(renewInterval));

    internal T?[] Objects { get; private set; } = [];

    internal void Renew() => this.Objects = UnityObject.FindObjectsByType<T>(FindObjectsSortMode.None);

    IEnumerator RenewObjects(float renewInterval) {
        WaitForSeconds waitForRenewInterval = new(renewInterval);

        while (true) {
            this.Renew();
            yield return waitForRenewInterval;
        }
    }
}
