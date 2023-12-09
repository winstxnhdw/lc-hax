using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static Vector3 Copy(Vector3 vector) {
        return new Vector3(
            vector.x,
            vector.y,
            vector.z
        );
    }

    public static GameObject Copy(Transform transform) {
        GameObject gameObject = new();
        gameObject.transform.position = Helper.Copy(transform.position);
        gameObject.transform.eulerAngles = Helper.Copy(transform.eulerAngles);

        return gameObject;
    }
}
