using System;

[AttributeUsage(AttributeTargets.Class)]
sealed class DebugCommandAttribute(string syntax) : Attribute {
    internal string Syntax { get; } = syntax;
}
