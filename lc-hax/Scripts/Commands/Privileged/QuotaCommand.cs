using Hax;

[PrivilegedCommand("/quota")]
public class QuotaCommand : ICommand {

    void SetQuota(ulong profit, ulong fulfilled = 0) {
        if (Helper.TimeOfDay != null) {
            Helper.TimeOfDay.profitQuota = (int)profit;
            Helper.TimeOfDay.quotaFulfilled = (int)fulfilled;
            Helper.TimeOfDay.UpdateProfitQuotaCurrentTime();
        }
    }

    public void Execute(StringArray args) {
        if (Helper.TimeOfDay == null) {
            Chat.Print("TimeOfDay helper is not available.");
            return;
        }

        if (args.Length < 1) {
            Chat.Print("Usage: /quota <amount> <fulfilled?>");
            return;
        }

        if (!ulong.TryParse(args[0], out ulong amount)) {
            Chat.Print("Invalid amount!");
            return;
        }

        ulong fulfilled = 0;
        if (args.Length > 1 && !ulong.TryParse(args[1], out fulfilled)) {
            Chat.Print("Invalid fulfilled amount, setting to 0.");
        }

        this.SetQuota(amount, fulfilled);
    }
}
