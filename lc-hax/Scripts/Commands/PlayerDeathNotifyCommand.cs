using Hax;

[Command("/warnofdeath")]
internal class PlayerDeathNotifyCommand : ICommand {
    public void Execute(StringArray _) {
        Setting.EnablePlayerDeathNotifications = !Setting.EnablePlayerDeathNotifications;
        Chat.Print($"Players Death Notifications: {(Setting.EnablePlayerDeathNotifications ? "Enabled" : "Disabled")}");
    }
}
