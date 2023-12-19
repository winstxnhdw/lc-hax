namespace Hax;

public static partial class Helper {
    public static void TriggerMine(this Landmine landmine) =>
        landmine.Reflect().InvokeInternalMethod("TriggerMineOnLocalClientByExiting");
}
