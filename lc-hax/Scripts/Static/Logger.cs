using System;
using System.IO;

internal static class Logger
{
    private const string LogFileName = "lc-hax.log";
    private static object LockObject { get; } = new();

    internal static void Write(string message)
    {
        Console.WriteLine(message);

        lock (LockObject)
        {
            var timeNow = DateTime.Now.ToString("dd-MM-yy HH:mm:ss");

            File.AppendAllText(
                LogFileName,
                $"[{timeNow}] {message}{Environment.NewLine}"
            );
        }
    }

    internal static void Write(Exception exception)
    {
        Write(exception.ToString());
    }
}