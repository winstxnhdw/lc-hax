#region

using System.Linq;
using Hax;

#endregion

interface IJetpack {
}

static class JetpackMixin {
    internal static JetpackItem[] GetAvailableJetpacks(this IJetpack _) =>
        Helper
            .FindObjects<JetpackItem>()
            .Where(jetpack => !jetpack.Reflect().GetInternalField<bool>("jetpackBroken"))
            .ToArray();

    internal static JetpackItem? GetAvailableJetpack(this IJetpack _) =>
        Helper
            .FindObjects<JetpackItem>()
            .First(jetpack => !jetpack.Reflect().GetInternalField<bool>("jetpackBroken"));
}
