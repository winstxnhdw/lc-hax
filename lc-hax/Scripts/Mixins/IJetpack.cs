using ZLinq;

interface IJetpack;

static class JetpackMixin {
    internal static JetpackItem[] GetAvailableJetpacks(this IJetpack _) =>
        Helper.FindObjects<JetpackItem>().AsValueEnumerable().Where(jetpack => !jetpack.jetpackBroken).ToArray();

    internal static JetpackItem? GetAvailableJetpack(this IJetpack _) =>
        Helper.FindObjects<JetpackItem>().First(jetpack => !jetpack.jetpackBroken);
}
