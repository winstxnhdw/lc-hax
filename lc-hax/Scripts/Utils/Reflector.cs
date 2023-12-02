using System;
using System.Reflection;

namespace Hax;

public class Reflector {
    const BindingFlags @internal = BindingFlags.NonPublic | BindingFlags.Instance;
    const BindingFlags @public = BindingFlags.Public | BindingFlags.Instance;
    const BindingFlags internalStatic = BindingFlags.NonPublic | BindingFlags.Static;
    const BindingFlags publicStatic = BindingFlags.Public | BindingFlags.Static;

    static BindingFlags InternalField => Reflector.@internal | BindingFlags.GetField;
    static BindingFlags InternalProperty => Reflector.@internal | BindingFlags.GetProperty;
    static BindingFlags InternalMethod => Reflector.@internal | BindingFlags.InvokeMethod;
    static BindingFlags InternalStaticField => Reflector.internalStatic | BindingFlags.GetField;
    static BindingFlags InternalStaticProperty => Reflector.internalStatic | BindingFlags.GetProperty;
    static BindingFlags InternalStaticMethod => Reflector.internalStatic | BindingFlags.InvokeMethod;

    static BindingFlags PublicField => Reflector.@public | BindingFlags.GetField;
    static BindingFlags PublicProperty => Reflector.@public | BindingFlags.GetProperty;
    static BindingFlags PublicMethod => Reflector.@public | BindingFlags.InvokeMethod;
    static BindingFlags PublicStaticField => Reflector.publicStatic | BindingFlags.GetField;
    static BindingFlags PublicStaticProperty => Reflector.publicStatic | BindingFlags.GetProperty;
    static BindingFlags PublicStaticMethod => Reflector.publicStatic | BindingFlags.InvokeMethod;

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

    public T? GetInternalField<T>(string variableName) => this.GetField<T>(variableName, Reflector.InternalField);

    public T? GetInternalStaticField<T>(string variableName) => this.GetField<T>(variableName, Reflector.InternalStaticField);

    public T? GetPublicField<T>(string variableName) => this.GetField<T>(variableName, Reflector.PublicField);

    public T? GetPublicStaticField<T>(string variableName) => this.GetField<T>(variableName, Reflector.PublicStaticField);

    public Reflector? GetInternalField(string variableName) {
        object? type = this.GetInternalField(variableName);
        return type == null ? null : new(type);
    }

    public Reflector? GetInternalStaticField(string variableName) {
        object? type = this.GetInternalStaticField(variableName);
        return type == null ? null : new(type);
    }

    public Reflector? GetPublicField(string variableName) {
        object? type = this.GetPublicField(variableName);
        return type == null ? null : new(type);
    }

    public Reflector? GetPublicStaticField(string variableName) {
        object? obj = this.GetPublicStaticField(variableName);
        return obj == null ? null : new(obj);
    }

    public Reflector? SetInternalField(string variableName, object value) => this.SetField(variableName, value, Reflector.InternalField);

    public Reflector? SetInternalStaticField(string variableName, object value) => this.SetField(variableName, value, Reflector.InternalStaticField);

    public Reflector? SetPublicField(string variableName, object value) => this.SetField(variableName, value, Reflector.PublicField);

    public Reflector? SetPublicStaticField(string variableName, object value) => this.SetField(variableName, value, Reflector.PublicStaticField);

    public Reflector? GetInternalProperty(string propertyName) => this.GetProperty(propertyName, Reflector.InternalProperty);

    public Reflector? GetPublicProperty(string propertyName) => this.GetProperty(propertyName, Reflector.PublicProperty);

    public Reflector? SetInternalProperty(string propertyName, object value) => this.SetProperty(propertyName, value, Reflector.InternalProperty);

    public Reflector? SetPublicProperty(string propertyName, object value) => this.SetProperty(propertyName, value, Reflector.PublicProperty);

    public T? InvokeInternalMethod<T>(string methodName, params object[] args) => this.InvokeMethod<T>(methodName, Reflector.InternalMethod, args);

    public T? InvokePublicMethod<T>(string methodName, params object[] args) => this.InvokeMethod<T>(methodName, Reflector.PublicMethod, args);

    public T? InvokePublicStaticMethod<T>(string methodName, params object[] args) => this.InvokeMethod<T>(methodName, Reflector.PublicStaticMethod, args);

    public Reflector? InvokeInternalMethod(string methodName, params object[] args) {
        object? obj = this.InvokeInternalMethod<object>(methodName, args);
        return obj == null ? null : new Reflector(obj);
    }

    public Reflector? InvokePublicMethod(string methodName, params object[] args) {
        object? obj = this.InvokePublicMethod<object>(methodName, args);
        return obj == null ? null : new Reflector(obj);
    }

    public Reflector? InvokePublicStaticMethod(string methodName, params object[] args) {
        object? obj = this.InvokePublicStaticMethod<object>(methodName, args);
        return obj == null ? null : new Reflector(obj);
    }

    public static Reflector Target(object obj) => new(obj);
}
