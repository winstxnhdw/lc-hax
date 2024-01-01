using System;

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute(string syntax) : Attribute {
    public string Syntax { get; } = syntax;
}
