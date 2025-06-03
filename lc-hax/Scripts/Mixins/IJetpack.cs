using System.Linq;

interface IJetpack { }

static class JetpackMixin {
    internal static JetpackItem[] GetAvailableJetpacks(this IJetpack _) =>
        [.. Helper.FindObjects<JetpackItem>().Where(jetpack => !jetpack.jetpackBroken)];

    internal static JetpackItem? GetAvailableJetpack(this IJetpack _) =>
        Helper.FindObjects<JetpackItem>().First(jetpack => !jetpack.jetpackBroken);
}
