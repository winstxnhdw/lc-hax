using Hax;

[Command("fakedeath")]
internal class FakeDeathCommand : ICommand {
    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;

        Setting.EnableFakeDeath = true;

        _ = player.Reflect().InvokeInternalMethod(
            "KillPlayerServerRpc",
            player.PlayerIndex(),
            false,
            Vector3.zero,
            CauseOfDeath.Unknown,
            0
        );
    }
}
