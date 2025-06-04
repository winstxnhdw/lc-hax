using System.Threading;
using System.Threading.Tasks;

[PrivilegedCommand("quota")]
class QuotaCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.TimeOfDay is not TimeOfDay timeOfDay) return;
        if (args.Length is 0) {
            Chat.Print("Usage: quota <amount> <fulfilled?>");
            return;
        }

        if (!ushort.TryParse(args[0], out ushort amount)) {
            Chat.Print($"Quota {nameof(amount)} must be a positive number!");
            return;
        }

        if (!args[1].TryParse(defaultValue: 0, result: out ushort fulfilled)) {
            Chat.Print($"The {nameof(fulfilled)} amount must be a positive number!");
            return;
        }

        timeOfDay.profitQuota = amount;
        timeOfDay.quotaFulfilled = fulfilled;
        timeOfDay.UpdateProfitQuotaCurrentTime();
    }
}
