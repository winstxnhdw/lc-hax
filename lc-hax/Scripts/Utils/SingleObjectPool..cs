using System.Collections;
using UnityObject = UnityEngine.Object;

internal class SingleObjectPool<T> where T : UnityObject {
    internal T? Object { get; private set; }

    internal SingleObjectPool(MonoBehaviour self, float renewInterval = 1.0f) => self.StartCoroutine(this.RenewObject(renewInterval));

    internal void Renew() => this.Object = UnityObject.FindAnyObjectByType<T>();

    IEnumerator RenewObject(float renewInterval) {
        WaitForSeconds waitForRenewInterval = new(renewInterval);

        while (true) {
            this.Renew();
            yield return waitForRenewInterval;
        }
    }
}
