#region

using System;

#endregion

[AttributeUsage(AttributeTargets.Class)]
class DebugCommandAttribute(string syntax) : Attribute {
    internal string Syntax { get; } = syntax;
}
