using Hax;

[PrivilegedCommand("/quota")]
public class QuotaCommand : ICommand {

    void SetQuota(int profit, int fulfilled = 0) {
        if (TimeOfDay.Instance == null) return;
        TimeOfDay.Instance.profitQuota = profit;
        TimeOfDay.Instance.quotaFulfilled = fulfilled;
        TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
    }


    public void Execute(StringArray args) {
        if (args.Length < 2) {
            Chat.Print("Usage: /quota <amount> <fulfilled?>");
            return;
        }

        if (!int.TryParse(args[0], out int amount)) {
            Chat.Print("Invalid amount!");
            return;
        }

        if (args.Length > 1) {
            if (!int.TryParse(args[1], out int fulfilled)) {
                Chat.Print("Invalid fulfilled amount!");
                return;
            }
            this.SetQuota(amount, fulfilled);
            return;
        }

        this.SetQuota(amount, 0);

    }
}
