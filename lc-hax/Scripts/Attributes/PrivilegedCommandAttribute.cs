using System;

[AttributeUsage(AttributeTargets.Class)]
public class PrivilegedCommandAttribute(string syntax) : Attribute {
    public string Syntax { get; } = syntax;
}
