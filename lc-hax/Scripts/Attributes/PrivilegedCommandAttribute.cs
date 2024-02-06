using System;

[AttributeUsage(AttributeTargets.Class)]
internal class PrivilegedCommandAttribute(string syntax) : Attribute {
    internal string Syntax { get; } = syntax;
}
