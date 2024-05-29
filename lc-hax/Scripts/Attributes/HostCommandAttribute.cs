using System;

[AttributeUsage(AttributeTargets.Class)]
internal class HostCommandAttribute(string syntax) : Attribute
{
    internal string Syntax { get; } = syntax;
}