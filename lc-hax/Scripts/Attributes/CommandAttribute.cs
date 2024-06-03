#region

using System;

#endregion

[AttributeUsage(AttributeTargets.Class)]
class CommandAttribute(string syntax) : Attribute {
    internal string Syntax { get; } = syntax;
}
