using System;

[AttributeUsage(AttributeTargets.Class)]
class CommandAttribute(string syntax) : Attribute {
    internal string Syntax { get; } = syntax;
}
