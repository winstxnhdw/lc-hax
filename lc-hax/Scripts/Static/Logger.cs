using System;
using System.IO;

internal static class Logger {
    const string logFileName = "lc-hax.log";
    static object LockObject { get; } = new();

    internal static void Write(string message) {
        lock (Logger.LockObject) {
            string timeNow = DateTime.Now.ToString("dd-MM-yy HH:mm:ss");

            File.AppendAllText(
                logFileName,
                $"[{timeNow}] {message}{Environment.NewLine}"
            );
        }
    }

    internal static void Write(Exception exception) => Logger.Write(exception.ToString());
}
