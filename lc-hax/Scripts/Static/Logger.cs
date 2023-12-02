using System;
using System.IO;

public static class Logger {
    const string logFileName = "lc-hax.log";
    static object LockObject { get; } = new();

    public static void Write(string message) {
        lock (Logger.LockObject) {
            File.AppendAllText(logFileName, $"{message}{Environment.NewLine}");
        }
    }
}
