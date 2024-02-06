using System;

[AttributeUsage(AttributeTargets.Class)]
internal class CommandAttribute(string syntax) : Attribute {
    internal string Syntax { get; } = syntax;
}
