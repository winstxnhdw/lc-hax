using System;

[AttributeUsage(AttributeTargets.Class)]
class DebugCommandAttribute(string syntax) : Attribute {
    internal string Syntax { get; } = syntax;
}
