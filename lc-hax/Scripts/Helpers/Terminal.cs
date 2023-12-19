namespace Hax;

public static partial class Helper {
    public static Terminal? Terminal => Helper.HUDManager?.Reflect().GetInternalField<Terminal>("terminalScript");
}
