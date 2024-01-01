using System;

[AttributeUsage(AttributeTargets.Class)]
public class DebugCommandAttribute(string syntax) : Attribute {
    public string Syntax { get; } = syntax;
}
