using System;

[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct)]
sealed class RequireNamedArgsAttribute : Attribute { }
