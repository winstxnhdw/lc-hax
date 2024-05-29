using System.Linq;
using Hax;

internal interface IJetpack
{
}

internal static class JetpackMixin
{
    internal static JetpackItem[] GetAvailableJetpacks(this IJetpack _)
    {
        return Helper
            .FindObjects<JetpackItem>()
            .Where(jetpack => !jetpack.Reflect().GetInternalField<bool>("jetpackBroken"))
            .ToArray();
    }

    internal static JetpackItem? GetAvailableJetpack(this IJetpack _)
    {
        return Helper
            .FindObjects<JetpackItem>()
            .First(jetpack => !jetpack.Reflect().GetInternalField<bool>("jetpackBroken"));
    }
}