#pragma warning disable CS9113

namespace System.Runtime.CompilerServices;

internal static class IsExternalInit { }

[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
internal sealed class RequiredMemberAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
internal sealed class CompilerFeatureRequiredAttribute(string name) : Attribute { }
