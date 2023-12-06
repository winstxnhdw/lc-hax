namespace Hax;

public static partial class Helpers {
    public static Terminal? Terminal => Helpers.HUDManager == null ? null : Reflector.Target(Helpers.HUDManager).GetInternalField<Terminal>("terminalScript");
}
