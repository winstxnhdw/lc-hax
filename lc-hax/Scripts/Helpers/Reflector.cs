namespace Hax;

public static partial class Helper {
    public static Reflector Reflect(this object obj) => Reflector.Target(obj);
}
