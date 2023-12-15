namespace Hax;

public static partial class Helper {
    public static void TriggerMine(this Landmine landmine) =>
        Reflector.Target(landmine).InvokeInternalMethod("TriggerMineOnLocalClientByExiting");
}
