using System;
using System.Reflection;

namespace Hax;

public class Reflector {
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

    Reflector? GetProperty(string propertyName, BindingFlags flags) {
        try {
            return new(
                this.ObjType.GetProperty(propertyName, flags).GetValue(this.Obj, null)
            );
        }

        catch (Exception) {
            return null;
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
            this.ObjType.GetProperty(propertyName, flags).SetValue(this.Obj, value, null);
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

    public T? GetInternalField<T>(string variableName) => this.GetField<T>(variableName, Reflector.internalField);

    public T? GetInternalStaticField<T>(string variableName) => this.GetField<T>(variableName, Reflector.internalStaticField);

    public Reflector? GetInternalField(string variableName) {
        object? type = this.GetInternalField(variableName);
        return type is null ? null : new(type);
    }

    public Reflector? GetInternalStaticField(string variableName) {
        object? type = this.GetInternalStaticField(variableName);
        return type is null ? null : new(type);
    }

    public Reflector? SetInternalField(string variableName, object value) => this.SetField(variableName, value, Reflector.internalField);

    public Reflector? SetInternalStaticField(string variableName, object value) => this.SetField(variableName, value, Reflector.internalStaticField);

    public Reflector? GetInternalProperty(string propertyName) => this.GetProperty(propertyName, Reflector.internalProperty);

    public Reflector? SetInternalProperty(string propertyName, object value) => this.SetProperty(propertyName, value, Reflector.internalProperty);

    public T? InvokeInternalMethod<T>(string methodName, params object[] args) => this.InvokeMethod<T>(methodName, Reflector.internalMethod, args);

    public Reflector? InvokeInternalMethod(string methodName, params object[] args) {
        object? obj = this.InvokeInternalMethod<object>(methodName, args);
        return obj is null ? null : new Reflector(obj);
    }

    public static Reflector Target(object obj) => new(obj);
}
