using System.Linq;
using Hax;

interface IJetpack { }

static class JetpackMixin {
    static JetpackItem[] Jetpacks { get; set; } = [];

    internal static JetpackItem[] GetAvailableJetpacks(this IJetpack _) {
        JetpackMixin.Jetpacks ??= Helper.FindObjects<JetpackItem>();

        return JetpackMixin
            .Jetpacks
            .Where(jetpack => !jetpack.Reflect().GetInternalField<bool>("jetpackBroken"))
            .ToArray();
    }

    internal static JetpackItem? GetAvailableJetpack(this IJetpack _) {
        JetpackMixin.Jetpacks ??= Helper.FindObjects<JetpackItem>();

        return JetpackMixin
            .Jetpacks
            .First(jetpack => !jetpack.Reflect().GetInternalField<bool>("jetpackBroken"));
    }
}
