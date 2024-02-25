using System;

[AttributeUsage(AttributeTargets.Class)]
class PrivilegedCommandAttribute(string syntax) : Attribute {
    internal string Syntax { get; } = syntax;
}
