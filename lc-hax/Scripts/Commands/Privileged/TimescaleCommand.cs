using Hax;

[PrivilegedCommand("timescale")]
internal class TimescaleCommand : ICommand {
    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: timescale <scale>");
            return;
        }

        if (!float.TryParse(args[0], out float timescale)) {
            Chat.Print("Invalid timescale!");
            return;
        }

        Time.timeScale = timescale;
    }
}
