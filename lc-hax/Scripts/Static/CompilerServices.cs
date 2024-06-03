#pragma warning disable CS9113

namespace System.Runtime.CompilerServices;

static class IsExternalInit {
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Struct,
    AllowMultiple = false, Inherited = false)]
sealed class RequiredMemberAttribute : Attribute {
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
sealed class CompilerFeatureRequiredAttribute(string name) : Attribute {
}
