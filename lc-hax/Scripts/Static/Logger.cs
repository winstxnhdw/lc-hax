using System;
using System.IO;

static class Logger {
    const string LogFileName = "lc-hax.log";
    static object LockObject { get; } = new();

    internal static void Write(string message) {
        Console.WriteLine(message);

        lock (Logger.LockObject) {
            string timeNow = DateTime.Now.ToString("dd-MM-yy HH:mm:ss");

            File.AppendAllText(
                LogFileName,
                $"[{timeNow}] {message}{Environment.NewLine}"
            );
        }
    }

    internal static void Write(Exception exception) => Logger.Write(exception.ToString());
}
