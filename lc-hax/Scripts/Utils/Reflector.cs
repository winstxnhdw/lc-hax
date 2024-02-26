using System;
using System.Reflection;
using System.Collections.Generic;

class Reflector {
    const BindingFlags PrivateOrInternal = BindingFlags.NonPublic | BindingFlags.Instance;
    const BindingFlags InternalStatic = BindingFlags.NonPublic | BindingFlags.Static;
    const BindingFlags InternalField = Reflector.PrivateOrInternal | BindingFlags.GetField;
    const BindingFlags InternalProperty = Reflector.PrivateOrInternal | BindingFlags.GetProperty;
    const BindingFlags InternalMethod = Reflector.PrivateOrInternal | BindingFlags.InvokeMethod;
    const BindingFlags InternalStaticField = Reflector.InternalStatic | BindingFlags.GetField;
    const BindingFlags InternalStaticProperty = Reflector.InternalStatic | BindingFlags.GetProperty;
    const BindingFlags InternalStaticMethod = Reflector.InternalStatic | BindingFlags.InvokeMethod;

    object Obj { get; }
    Type ObjType { get; }

    Dictionary<string, MemberInfo> Cache { get; } = [];

    internal Reflector(object obj) {
        this.Obj = obj;
        this.ObjType = obj.GetType();
    }

    T? GetField<T>(string variableName, BindingFlags flags) {
        try {
            if (!this.Cache.TryGetValue(variableName, out MemberInfo field)) {
                this.Cache[variableName] = field = this.ObjType.GetField(variableName, flags);
            }

            object? result = (field as FieldInfo)?.GetValue(this.Obj);
            return result is null ? default : (T)result;
        }

        catch (InvalidCastException) {
            return default;
        }
    }

    T? GetProperty<T>(string propertyName, BindingFlags flags) {
        try {
            if (!this.Cache.TryGetValue(propertyName, out MemberInfo property)) {
                this.Cache[propertyName] = property = this.ObjType.GetProperty(propertyName, flags);
            }

            object? result = (property as PropertyInfo)?.GetValue(this.Obj);
            return result is null ? default : (T)result;
        }

        catch (InvalidCastException) {
            return default;
        }
    }

    Reflector? SetField(string variableName, object value, BindingFlags flags) {
        try {
            if (!this.Cache.TryGetValue(variableName, out MemberInfo field)) {
                this.Cache[variableName] = field = this.ObjType.GetField(variableName, flags);
            }

            (field as FieldInfo)?.SetValue(this.Obj, value);
            return this;
        }

        catch (Exception) {
            return null;
        }
    }

    Reflector? SetProperty(string propertyName, object value, BindingFlags flags) {
        try {
            if (!this.Cache.TryGetValue(propertyName, out MemberInfo property)) {
                this.Cache[propertyName] = property = this.ObjType.GetProperty(propertyName, flags);
            }

            (property as PropertyInfo)?.SetValue(this.Obj, value);
            return this;
        }

        catch (Exception) {
            return null;
        }
    }

    T? InvokeMethod<T>(string methodName, BindingFlags flags, params object[] args) {
        try {
            if (!this.Cache.TryGetValue(methodName, out MemberInfo method)) {
                this.Cache[methodName] = method = this.ObjType.GetMethod(methodName, flags);
            }

            object? result = (method as MethodInfo)?.Invoke(this.Obj, args);
            return result is null ? default : (T)result;
        }

        catch (InvalidCastException) {
            return default;
        }
    }

    internal T? GetInternalField<T>(string variableName) => this.GetField<T>(variableName, Reflector.InternalField);

    internal T? GetInternalStaticField<T>(string variableName) => this.GetField<T>(variableName, Reflector.InternalStaticField);

    internal T? GetInternalProperty<T>(string propertyName) => this.GetProperty<T>(propertyName, Reflector.InternalProperty);

    internal T? InvokeInternalMethod<T>(string methodName, params object[] args) => this.InvokeMethod<T>(methodName, Reflector.InternalMethod, args);

    internal T? InvokeInternalStaticMethod<T>(string methodName, params object[] args) => this.InvokeMethod<T>(methodName, Reflector.InternalStaticMethod, args);

    internal Reflector? SetInternalField(string variableName, object value) => this.SetField(variableName, value, Reflector.InternalField);

    internal Reflector? SetInternalStaticField(string variableName, object value) => this.SetField(variableName, value, Reflector.InternalStaticField);

    internal Reflector? SetInternalProperty(string propertyName, object value) => this.SetProperty(propertyName, value, Reflector.InternalProperty);

    internal Reflector? GetInternalField(string variableName) => this.GetInternalField<object>(variableName)?.Reflect();

    internal Reflector? GetInternalStaticField(string variableName) => this.GetInternalStaticField<object>(variableName)?.Reflect();

    internal Reflector? GetInternalProperty(string propertyName) => this.GetInternalProperty<object>(propertyName)?.Reflect();

    internal Reflector? InvokeInternalMethod(string methodName, params object[] args) => this.InvokeInternalMethod<object>(methodName, args)?.Reflect();

    internal Reflector? InvokeInternalStaticMethod(string methodName, params object[] args) => this.InvokeInternalStaticMethod<object>(methodName, args)?.Reflect();
}

static class ReflectorExtensions {
    internal static Reflector Reflect(this object obj) => new(obj);
}
