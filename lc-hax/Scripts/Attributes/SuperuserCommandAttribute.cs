using System;

[AttributeUsage(AttributeTargets.Class)]
public class SuperuserCommandAttribute(string syntax) : Attribute {
    public string Syntax { get; } = syntax;
}
