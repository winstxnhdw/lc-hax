#region

using System;

#endregion

[AttributeUsage(AttributeTargets.Class)]
class HostCommandAttribute(string syntax) : Attribute {
    internal string Syntax { get; } = syntax;
}
