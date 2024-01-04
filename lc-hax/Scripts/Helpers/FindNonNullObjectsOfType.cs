using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static IEnumerable<T> FindNonNullObjectsOfType<T>() where T : Object => Object.FindObjectsOfType<T>().Where(obj => obj != null);
}
