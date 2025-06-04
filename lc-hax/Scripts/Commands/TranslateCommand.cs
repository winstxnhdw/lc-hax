using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("translate")]
class TranslateCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (args.Length < 2) {
            Chat.Print("Usages: translate <language> <text>");
            return;
        }

        string? language = args[0];

        if (string.IsNullOrWhiteSpace(language)) {
            Chat.Print($"Invalid {nameof(language)}!");
            return;
        }

        using Translator translator = new(language, cancellationToken);
        string? translatedText = await translator.Translate(string.Join(' ', args[1..]));

        if (string.IsNullOrWhiteSpace(translatedText)) {
            Chat.Print("Failed to translate the text!");
            return;
        }

        Helper.HUDManager?.AddPlayerChatMessageServerRpc(translatedText, player.PlayerIndex());
    }
}
