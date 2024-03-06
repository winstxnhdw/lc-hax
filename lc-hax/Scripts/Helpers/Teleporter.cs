using System;
using UnityEngine;

namespace Hax;

static partial class Helper {
    internal static ShipTeleporter?[] ShipTeleporters => HaxObjects.Instance?.ShipTeleporters?.Objects ?? [];

}
