using System;

[AttributeUsage(AttributeTargets.Class)]
internal class DebugCommandAttribute(string syntax) : Attribute {
    internal string Syntax { get; } = syntax;
}
