namespace Hax;

public static partial class Helper {
    public static Terminal? Terminal => Helper.HUDManager == null ? null : Reflector.Target(Helper.HUDManager).GetInternalField<Terminal>("terminalScript");
}
