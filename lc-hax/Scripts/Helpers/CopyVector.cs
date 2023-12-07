using UnityEngine;

namespace Hax;

public static partial class Helpers {
    public static Vector3 CopyVector(Vector3 vector3) {
        float x = vector3.x;
        float y = vector3.y;
        float z = vector3.z;
        return new Vector3(x, y, z);
    }
}
