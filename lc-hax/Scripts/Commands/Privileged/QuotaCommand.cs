using Hax;

[PrivilegedCommand("/quota")]
public class QuotaCommand : ICommand {

    void SetQuota(uint profit, uint fulfilled = 0) {
        if (Helper.TimeOfDay != null) {
            Helper.TimeOfDay.profitQuota = (int)profit; // Cast to int if necessary
            Helper.TimeOfDay.quotaFulfilled = (int)fulfilled; // Cast to int if necessary
            Helper.TimeOfDay.UpdateProfitQuotaCurrentTime();
        }
    }

    public void Execute(StringArray args) {
        if (Helper.TimeOfDay == null) return;
        if (args.Length < 2) {
            Chat.Print("Usage: /quota <amount> <fulfilled?>");
            return;
        }

        if (!uint.TryParse(args[0], out uint amount)) {
            Chat.Print("Invalid amount!");
            return;
        }

        if (args.Length > 1) {
            if (!uint.TryParse(args[1], out uint fulfilled)) {
                Chat.Print("Invalid fulfilled amount!");
                return;
            }

            this.SetQuota(amount, fulfilled);
            return;
        }

        this.SetQuota(amount, 0);
    }
}
