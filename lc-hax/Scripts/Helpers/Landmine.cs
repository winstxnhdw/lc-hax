namespace Hax;

static partial class Helper {
    internal static void TriggerMine(this Landmine landmine) =>
        landmine.Reflect().InvokeInternalMethod("TriggerMineOnLocalClientByExiting");
}
