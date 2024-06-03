#region

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

class Reflector<R> {
    const BindingFlags PrivateOrInternal = BindingFlags.NonPublic | BindingFlags.Instance;
    const BindingFlags InternalStatic = BindingFlags.NonPublic | BindingFlags.Static;
    const BindingFlags InternalField = PrivateOrInternal | BindingFlags.GetField;
    const BindingFlags InternalProperty = PrivateOrInternal | BindingFlags.GetProperty;
    const BindingFlags InternalMethod = PrivateOrInternal | BindingFlags.InvokeMethod;
    const BindingFlags InternalStaticField = InternalStatic | BindingFlags.GetField;
    const BindingFlags InternalStaticProperty = InternalStatic | BindingFlags.GetProperty;
    const BindingFlags InternalStaticMethod = InternalStatic | BindingFlags.InvokeMethod;

    internal Reflector(R obj) {
        this.Object = obj;
        this.ObjectType = typeof(R);
    }

    R Object { get; }
    Type ObjectType { get; }

    Dictionary<string, MemberInfo> Cache { get; } = [];

    FieldInfo CachedField(string variableName, BindingFlags flags) {
        if (!this.Cache.TryGetValue(variableName, out MemberInfo? field))
            this.Cache[variableName] = field = this.ObjectType.GetField(variableName, flags);

        return (FieldInfo)field;
    }

    PropertyInfo CachedProperty(string propertyName, BindingFlags flags) {
        if (!this.Cache.TryGetValue(propertyName, out MemberInfo? property))
            this.Cache[propertyName] = property = this.ObjectType.GetProperty(propertyName, flags);

        return (PropertyInfo)property;
    }

    MethodInfo CachedMethod(string methodName, BindingFlags flags) {
        if (!this.Cache.TryGetValue(methodName, out MemberInfo? method))
            this.Cache[methodName] = method = this.ObjectType.GetMethod(methodName, flags);

        return (MethodInfo)method;
    }

    T? GetField<T>(string variableName, BindingFlags flags) {
        try {
            return (T)this.CachedField(variableName, flags).GetValue(this.Object);
        }

        catch (InvalidCastException) {
            return default;
        }
    }

    T? GetProperty<T>(string propertyName, BindingFlags flags) {
        try {
            return (T)this.CachedProperty(propertyName, flags).GetValue(this.Object);
        }

        catch (InvalidCastException) {
            return default;
        }
    }

    Reflector<R>? SetField(string variableName, object value, BindingFlags flags) {
        try {
            this.CachedField(variableName, flags).SetValue(this.Object, value);
            return this;
        }

        catch (Exception) {
            return null;
        }
    }

    Reflector<R>? SetProperty(string propertyName, object value, BindingFlags flags) {
        try {
            this.CachedProperty(propertyName, flags).SetValue(this.Object, value);
            return this;
        }

        catch (Exception) {
            return null;
        }
    }

    T? InvokeMethod<T>(string methodName, BindingFlags flags, params object[] args) {
        try {
            return (T)this.CachedMethod(methodName, flags).Invoke(this.Object, args);
        }

        catch (InvalidCastException) {
            return default;
        }
    }

    internal T? GetInternalField<T>(string variableName) => this.GetField<T>(variableName, InternalField);

    internal T? GetInternalStaticField<T>(string variableName) => this.GetField<T>(variableName, InternalStaticField);

    internal T? GetInternalProperty<T>(string propertyName) => this.GetProperty<T>(propertyName, InternalProperty);

    internal T? InvokeInternalMethod<T>(string methodName, params object[] args) =>
        this.InvokeMethod<T>(methodName, InternalMethod, args);

    internal T? InvokeInternalStaticMethod<T>(string methodName, params object[] args) =>
        this.InvokeMethod<T>(methodName, InternalStaticMethod, args);

    internal Reflector<R>? SetInternalField(string variableName, object value) =>
        this.SetField(variableName, value, InternalField);

    internal Reflector<R>? SetInternalStaticField(string variableName, object value) =>
        this.SetField(variableName, value, InternalStaticField);

    internal Reflector<R>? SetInternalProperty(string propertyName, object value) =>
        this.SetProperty(propertyName, value, InternalProperty);

    internal Reflector<R>? GetInternalField(string variableName) => this.GetInternalField<R>(variableName)?.Reflect();

    internal Reflector<R>? GetInternalStaticField(string variableName) =>
        this.GetInternalStaticField<R>(variableName)?.Reflect();

    internal Reflector<R>? GetInternalProperty(string propertyName) =>
        this.GetInternalProperty<R>(propertyName)?.Reflect();

    internal Reflector<R>? InvokeInternalMethod(string methodName, params object[] args) =>
        this.InvokeInternalMethod<R>(methodName, args)?.Reflect();

    internal Reflector<R>? InvokeInternalStaticMethod(string methodName, params object[] args) =>
        this.InvokeInternalStaticMethod<R>(methodName, args)?.Reflect();
}

static class ReflectorExtensions {
    internal static Reflector<R> Reflect<R>(this R obj) => new(obj);
}
