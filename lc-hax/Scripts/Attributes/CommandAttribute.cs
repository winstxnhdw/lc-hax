using System;

[AttributeUsage(AttributeTargets.Class)]
sealed class CommandAttribute(string syntax) : Attribute {
    internal string Syntax { get; } = syntax;
}
