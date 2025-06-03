using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("translate")]
class TranslateCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (args.Length < 2) {
            Chat.Print("Usages: translate <language> <text>");
            return;
        }

        using Translator translator = new(args[0], cancellationToken);
        string? translatedText = await translator.Translate(string.Join(' ', args[1..]));

        if (string.IsNullOrWhiteSpace(translatedText)) {
            Chat.Print("Failed to translate the text!");
            return;
        }

        Helper.HUDManager?.AddPlayerChatMessageServerRpc(translatedText, player.PlayerIndex());
    }
}
