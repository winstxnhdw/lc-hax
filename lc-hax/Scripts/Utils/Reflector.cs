using System;
using System.Collections.Generic;
using System.Reflection;

internal class Reflector<R>
{
    private const BindingFlags PrivateOrInternal = BindingFlags.NonPublic | BindingFlags.Instance;
    private const BindingFlags InternalStatic = BindingFlags.NonPublic | BindingFlags.Static;
    private const BindingFlags InternalField = PrivateOrInternal | BindingFlags.GetField;
    private const BindingFlags InternalProperty = PrivateOrInternal | BindingFlags.GetProperty;
    private const BindingFlags InternalMethod = PrivateOrInternal | BindingFlags.InvokeMethod;
    private const BindingFlags InternalStaticField = InternalStatic | BindingFlags.GetField;
    private const BindingFlags InternalStaticProperty = InternalStatic | BindingFlags.GetProperty;
    private const BindingFlags InternalStaticMethod = InternalStatic | BindingFlags.InvokeMethod;

    internal Reflector(R obj)
    {
        Object = obj;
        ObjectType = typeof(R);
    }

    private R Object { get; }
    private Type ObjectType { get; }

    private Dictionary<string, MemberInfo> Cache { get; } = [];

    private FieldInfo CachedField(string variableName, BindingFlags flags)
    {
        if (!Cache.TryGetValue(variableName, out var field))
            Cache[variableName] = field = ObjectType.GetField(variableName, flags);

        return (FieldInfo)field;
    }

    private PropertyInfo CachedProperty(string propertyName, BindingFlags flags)
    {
        if (!Cache.TryGetValue(propertyName, out var property))
            Cache[propertyName] = property = ObjectType.GetProperty(propertyName, flags);

        return (PropertyInfo)property;
    }

    private MethodInfo CachedMethod(string methodName, BindingFlags flags)
    {
        if (!Cache.TryGetValue(methodName, out var method))
            Cache[methodName] = method = ObjectType.GetMethod(methodName, flags);

        return (MethodInfo)method;
    }

    private T? GetField<T>(string variableName, BindingFlags flags)
    {
        try
        {
            return (T)CachedField(variableName, flags).GetValue(Object);
        }

        catch (InvalidCastException)
        {
            return default;
        }
    }

    private T? GetProperty<T>(string propertyName, BindingFlags flags)
    {
        try
        {
            return (T)CachedProperty(propertyName, flags).GetValue(Object);
        }

        catch (InvalidCastException)
        {
            return default;
        }
    }

    private Reflector<R>? SetField(string variableName, object value, BindingFlags flags)
    {
        try
        {
            CachedField(variableName, flags).SetValue(Object, value);
            return this;
        }

        catch (Exception)
        {
            return null;
        }
    }

    private Reflector<R>? SetProperty(string propertyName, object value, BindingFlags flags)
    {
        try
        {
            CachedProperty(propertyName, flags).SetValue(Object, value);
            return this;
        }

        catch (Exception)
        {
            return null;
        }
    }

    private T? InvokeMethod<T>(string methodName, BindingFlags flags, params object[] args)
    {
        try
        {
            return (T)CachedMethod(methodName, flags).Invoke(Object, args);
        }

        catch (InvalidCastException)
        {
            return default;
        }
    }

    internal T? GetInternalField<T>(string variableName)
    {
        return GetField<T>(variableName, InternalField);
    }

    internal T? GetInternalStaticField<T>(string variableName)
    {
        return GetField<T>(variableName, InternalStaticField);
    }

    internal T? GetInternalProperty<T>(string propertyName)
    {
        return GetProperty<T>(propertyName, InternalProperty);
    }

    internal T? InvokeInternalMethod<T>(string methodName, params object[] args)
    {
        return InvokeMethod<T>(methodName, InternalMethod, args);
    }

    internal T? InvokeInternalStaticMethod<T>(string methodName, params object[] args)
    {
        return InvokeMethod<T>(methodName, InternalStaticMethod, args);
    }

    internal Reflector<R>? SetInternalField(string variableName, object value)
    {
        return SetField(variableName, value, InternalField);
    }

    internal Reflector<R>? SetInternalStaticField(string variableName, object value)
    {
        return SetField(variableName, value, InternalStaticField);
    }

    internal Reflector<R>? SetInternalProperty(string propertyName, object value)
    {
        return SetProperty(propertyName, value, InternalProperty);
    }

    internal Reflector<R>? GetInternalField(string variableName)
    {
        return GetInternalField<R>(variableName)?.Reflect();
    }

    internal Reflector<R>? GetInternalStaticField(string variableName)
    {
        return GetInternalStaticField<R>(variableName)?.Reflect();
    }

    internal Reflector<R>? GetInternalProperty(string propertyName)
    {
        return GetInternalProperty<R>(propertyName)?.Reflect();
    }

    internal Reflector<R>? InvokeInternalMethod(string methodName, params object[] args)
    {
        return InvokeInternalMethod<R>(methodName, args)?.Reflect();
    }

    internal Reflector<R>? InvokeInternalStaticMethod(string methodName, params object[] args)
    {
        return InvokeInternalStaticMethod<R>(methodName, args)?.Reflect();
    }
}

internal static class ReflectorExtensions
{
    internal static Reflector<R> Reflect<R>(this R obj)
    {
        return new Reflector<R>(obj);
    }
}