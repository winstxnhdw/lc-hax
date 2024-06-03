#region

using System.Collections;
using UnityEngine;
using UnityObject = UnityEngine.Object;

#endregion

class SingleObjectPool<T> where T : UnityObject {
    internal SingleObjectPool(MonoBehaviour self, float renewInterval = 1.0f) =>
        self.StartCoroutine(this.RenewObject(renewInterval));

    internal T? Object { get; private set; }

    internal void Renew() => this.Object = UnityObject.FindAnyObjectByType<T>();

    IEnumerator RenewObject(float renewInterval) {
        WaitForSeconds waitForRenewInterval = new(renewInterval);

        while (true) {
            this.Renew();
            yield return waitForRenewInterval;
        }
    }
}
