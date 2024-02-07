using Hax;

[PrivilegedCommand("/quota")]
public class QuotaCommand : ICommand {
    public void Execute(StringArray args) {
        if (Helper.TimeOfDay is not TimeOfDay timeOfDay) return;
        if (args.Length < 1) {
            Chat.Print("Usage: /quota <amount> <fulfilled?>");
            return;
        }

        if (!ushort.TryParse(args[0], out ushort amount)) {
            Chat.Print("Invalid amount!");
            return;
        }

        if (args.Length > 1 && !ushort.TryParse(args[1], out ushort fulfilled)) {
            Chat.Print("Invalid fulfilled amount, setting to 0.");
        }

        timeOfDay.profitQuota = amount;
        timeOfDay.quotaFulfilled = fulfilled;
        timeOfDay.UpdateProfitQuotaCurrentTime();
    }
}
