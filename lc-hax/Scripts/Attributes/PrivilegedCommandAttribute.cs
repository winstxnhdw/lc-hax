using System;

[AttributeUsage(AttributeTargets.Class)]
sealed class PrivilegedCommandAttribute(string syntax) : Attribute {
    internal string Syntax { get; } = syntax;
}
