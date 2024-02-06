using System;
using System.Reflection;

internal class Reflector {
    const BindingFlags privateOrInternal = BindingFlags.NonPublic | BindingFlags.Instance;
    const BindingFlags internalStatic = BindingFlags.NonPublic | BindingFlags.Static;
    const BindingFlags internalField = Reflector.privateOrInternal | BindingFlags.GetField;
    const BindingFlags internalProperty = Reflector.privateOrInternal | BindingFlags.GetProperty;
    const BindingFlags internalMethod = Reflector.privateOrInternal | BindingFlags.InvokeMethod;
    const BindingFlags internalStaticField = Reflector.internalStatic | BindingFlags.GetField;
    const BindingFlags internalStaticProperty = Reflector.internalStatic | BindingFlags.GetProperty;
    const BindingFlags internalStaticMethod = Reflector.internalStatic | BindingFlags.InvokeMethod;

    object Obj { get; }
    Type ObjType { get; }

    Reflector(object obj) {
        this.Obj = obj;
        this.ObjType = obj.GetType();
    }

    T? GetField<T>(string variableName, BindingFlags flags) {
        try {
            return (T)this.ObjType.GetField(variableName, flags).GetValue(this.Obj);
        }

        catch (InvalidCastException) {
            return default;
        }
    }

    T? GetProperty<T>(string propertyName, BindingFlags flags) {
        try {
            return (T)this.ObjType.GetProperty(propertyName, flags).GetValue(this.Obj);
        }

        catch (InvalidCastException) {
            return default;
        }
    }

    Reflector? SetField(string variableName, object value, BindingFlags flags) {
        try {
            this.ObjType.GetField(variableName, flags).SetValue(this.Obj, value);
            return this;
        }

        catch (Exception) {
            return null;
        }
    }

    Reflector? SetProperty(string propertyName, object value, BindingFlags flags) {
        try {
            this.ObjType.GetProperty(propertyName, flags).SetValue(this.Obj, value);
            return this;
        }

        catch (Exception) {
            return null;
        }
    }

    T? InvokeMethod<T>(string methodName, BindingFlags flags, params object[] args) {
        try {
            return (T)this.ObjType.GetMethod(methodName, flags).Invoke(this.Obj, args);
        }

        catch (InvalidCastException) {
            return default;
        }
    }

    internal T? GetInternalField<T>(string variableName) => this.GetField<T>(variableName, Reflector.internalField);

    internal T? GetInternalStaticField<T>(string variableName) => this.GetField<T>(variableName, Reflector.internalStaticField);

    internal T? GetInternalProperty<T>(string propertyName) => this.GetProperty<T>(propertyName, Reflector.internalProperty);

    internal T? InvokeInternalMethod<T>(string methodName, params object[] args) => this.InvokeMethod<T>(methodName, Reflector.internalMethod, args);

    internal T? InvokeInternalStaticMethod<T>(string methodName, params object[] args) => this.InvokeMethod<T>(methodName, Reflector.internalStaticMethod, args);

    internal Reflector? SetInternalField(string variableName, object value) => this.SetField(variableName, value, Reflector.internalField);

    internal Reflector? SetInternalStaticField(string variableName, object value) => this.SetField(variableName, value, Reflector.internalStaticField);

    internal Reflector? SetInternalProperty(string propertyName, object value) => this.SetProperty(propertyName, value, Reflector.internalProperty);

    internal Reflector? GetInternalField(string variableName) => this.GetInternalField<object>(variableName)?.Reflect();

    internal Reflector? GetInternalStaticField(string variableName) => this.GetInternalStaticField<object>(variableName)?.Reflect();

    internal Reflector? GetInternalProperty(string propertyName) => this.GetInternalProperty<object>(propertyName)?.Reflect();

    internal Reflector? InvokeInternalMethod(string methodName, params object[] args) => this.InvokeInternalMethod<object>(methodName, args)?.Reflect();

    internal Reflector? InvokeInternalStaticMethod(string methodName, params object[] args) => this.InvokeInternalStaticMethod<object>(methodName, args)?.Reflect();

    internal static Reflector Target(object obj) => new(obj);
}

internal static class ReflectorExtensions {
    internal static Reflector Reflect(this object obj) => Reflector.Target(obj);
}
