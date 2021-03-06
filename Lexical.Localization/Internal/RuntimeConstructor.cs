﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           10.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Lexical.Localization.Internal
{
    // Example
    /* RuntimeConstructor<int, IList> listConstructor = new RuntimeConstructor<int, IList>(typeof(List<>));
     * 
     * Func<int, IList> constructor = listConstructor.GetOrCreateConstructor();
     * IList x = constructor.Create(256);
     * 
     * IList x2 = listConstructor.Create(typeof(byte), 256);
     */

    /// <summary>
    /// This class constructs generic types with runtime parametrization.
    /// This version uses one parameter constructor.
    /// 
    /// </summary>
    /// <typeparam name="ReturnType"></typeparam>
    public class RuntimeConstructor<ReturnType>
    {
        /// <summary>
        /// A generic type with one generic parameter, for example <see cref="List{T}"/>.
        /// 
        /// Must be assignable to ReturnType.
        /// </summary>
        Type genericType;

        /// <summary>
        /// Cache of constructors.
        /// 
        /// Argument of generic type is kept with weak key in case the future .NET Core can unload assemblies.
        /// We don't want to keep hard reference in cache that would prevent unloading. Perhaps that delegate will prevent anyway.
        /// </summary>
        ConditionalWeakTable<Type, Func<ReturnType>> constructorCache = new ConditionalWeakTable<Type, Func<ReturnType>>();

        /// <summary>
        /// A delegate for cache.
        /// </summary>
        ConditionalWeakTable<Type, Func<ReturnType>>.CreateValueCallback cacheCallback;

        /// <summary>
        /// Array of constructor arguments
        /// </summary>
        Type[] constructorParamTypes = new Type[] { };

        /// <summary>
        /// Array of constructor parameters
        /// </summary>
        ParameterExpression[] constructorParams = new ParameterExpression[0];

        /// <summary>
        /// Create object that can create constructors generic types with run-time parametrisation.
        /// </summary>
        /// <param name="genericType">A generic type with one generic parameter, for example <see cref="List{T}"/></param>
        public RuntimeConstructor(Type genericType)
        {
            // Assert
            this.genericType = genericType ?? throw new ArgumentNullException(nameof(genericType));
            Type[] genericArguments = genericType.GetGenericArguments();
            if (genericArguments.Length != 1 || genericArguments[0].IsGenericType) throw new ArgumentException($"Needs exactly one generic argument. {genericType.FullName} doesn't match criteria for {GetType().FullName}");

            // Create ConditionalWeakTable.CreateValueCallback delegate instance
            cacheCallback = CreateConstructor;
        }

        /// <summary>
        /// Creates new constructor delegate for GenericType{t}. 
        /// Does not use cache.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Func<ReturnType> CreateConstructor(Type t)
        {
            Type runtimeType = genericType.MakeGenericType(t);
            ConstructorInfo ci = runtimeType.GetConstructor(constructorParamTypes);
            if (ci == null) throw new Exception($"{GetType().FullName}: Could not find constructor for {runtimeType.FullName}({String.Join(", ", constructorParamTypes.Select(_ => _.FullName))})");

            // Create delegate
            return (Func<ReturnType>)Expression.Lambda(typeof(Func<ReturnType>), Expression.New(ci, constructorParams), constructorParams).Compile();
        }

        /// <summary>
        /// Get or create constructor delegate for GenericType{t}.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Func<ReturnType> GetOrCreateConstructor(Type t)
            => constructorCache.GetValue(t, cacheCallback);

        /// <summary>
        /// Create new instance of runtime type and arg0.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public ReturnType Create(Type t)
            => GetOrCreateConstructor(t)();
    }

    /// <summary>
    /// This class constructs generic types with runtime parametrization.
    /// This version uses one parameter constructor.
    /// 
    /// </summary>
    /// <typeparam name="ReturnType"></typeparam>
    /// <typeparam name="A0">First argument type of the constructor</typeparam>
    public class RuntimeConstructor<A0, ReturnType>
    {
        /// <summary>
        /// A generic type with one generic parameter, for example <see cref="List{T}"/>
        /// 
        /// Must be assignable to ReturnType.
        /// </summary>
        Type genericType;

        /// <summary>
        /// Cache of constructors.
        /// 
        /// Argument of generic type is kept with weak key in case the future .NET Core can unload assemblies.
        /// We don't want to keep hard reference in cache that would prevent unloading. Perhaps that delegate will prevent anyway.
        /// </summary>
        ConditionalWeakTable<Type, Func<A0, ReturnType>> constructorCache = new ConditionalWeakTable<Type, Func<A0, ReturnType>>();

        /// <summary>
        /// A delegate for cache.
        /// </summary>
        ConditionalWeakTable<Type, Func<A0, ReturnType>>.CreateValueCallback cacheCallback;

        /// <summary>
        /// Array of constructor arguments
        /// </summary>
        Type[] constructorParamTypes = new Type[] { typeof(A0) };

        /// <summary>
        /// Array of constructor parameters
        /// </summary>
        ParameterExpression[] constructorParams = new ParameterExpression[] { Expression.Parameter(typeof(A0)) };

        /// <summary>
        /// Create object that can create constructors generic types with run-time parametrisation.
        /// </summary>
        /// <param name="genericType">A generic type with one generic parameter, for example <see cref="List{T}"/></param>
        public RuntimeConstructor(Type genericType)
        {
            // Assert
            this.genericType = genericType ?? throw new ArgumentNullException(nameof(genericType));
            Type[] genericArguments = genericType.GetGenericArguments();
            if (genericArguments.Length != 1 || genericArguments[0].IsGenericType) throw new ArgumentException($"Needs exactly one generic argument. {genericType.FullName} doesn't match criteria for {GetType().FullName}");

            // Create ConditionalWeakTable.CreateValueCallback delegate instance
            cacheCallback = CreateConstructor;
        }

        /// <summary>
        /// Creates new constructor delegate for GenericType{t}. 
        /// Does not use cache.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Func<A0, ReturnType> CreateConstructor(Type t)
        {
            Type runtimeType = genericType.MakeGenericType(t);
            ConstructorInfo ci = runtimeType.GetConstructor(constructorParamTypes);
            if (ci == null) throw new Exception($"{GetType().FullName}: Could not find constructor for {runtimeType.FullName}({String.Join(", ", constructorParamTypes.Select(_=>_.FullName))})");

            // Create delegate
            return (Func<A0, ReturnType>) Expression.Lambda(typeof(Func<A0, ReturnType>), Expression.New(ci, constructorParams), constructorParams).Compile();
        }

        /// <summary>
        /// Get or create constructor delegate for GenericType{t}.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Func<A0, ReturnType> GetOrCreateConstructor(Type t)
            => constructorCache.GetValue(t, cacheCallback);

        /// <summary>
        /// Create new instance of runtime type and arg0.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="arg0"></param>
        /// <returns></returns>
        public ReturnType Create(Type t, A0 arg0)
            => GetOrCreateConstructor(t)(arg0);
    }

    /// <summary>
    /// This class constructs generic types with runtime parametrization.
    /// This version uses one parameter constructor.
    /// 
    /// </summary>
    /// <typeparam name="ReturnType"></typeparam>
    /// <typeparam name="A0">First argument type of the constructor</typeparam>
    /// <typeparam name="A1">Second argument type of the constructor</typeparam>
    public class RuntimeConstructor<A0, A1, ReturnType>
    {
        /// <summary>
        /// A generic type with one generic parameter, for example <see cref="List{T}"/>
        /// 
        /// Must be assignable to ReturnType.
        /// </summary>
        Type genericType;

        /// <summary>
        /// Cache of constructors.
        /// 
        /// Argument of generic type is kept with weak key in case the future .NET Core can unload assemblies.
        /// We don't want to keep hard reference in cache that would prevent unloading. Perhaps that delegate will prevent anyway.
        /// </summary>
        ConditionalWeakTable<Type, Func<A0, A1, ReturnType>> constructorCache = new ConditionalWeakTable<Type, Func<A0, A1, ReturnType>>();

        /// <summary>
        /// A delegate for cache.
        /// </summary>
        ConditionalWeakTable<Type, Func<A0, A1, ReturnType>>.CreateValueCallback cacheCallback;

        /// <summary>
        /// Array of constructor arguments
        /// </summary>
        Type[] constructorParamTypes = new Type[] { typeof(A0), typeof(A1) };

        /// <summary>
        /// Array of constructor parameters
        /// </summary>
        ParameterExpression[] constructorParams = new ParameterExpression[] { Expression.Parameter(typeof(A0)), Expression.Parameter(typeof(A1)) };

        /// <summary>
        /// Create object that can create constructors generic types with run-time parametrisation.
        /// </summary>
        /// <param name="genericType">A generic type with one generic parameter, for example <see cref="List{T}"/></param>
        public RuntimeConstructor(Type genericType)
        {
            // Assert
            this.genericType = genericType ?? throw new ArgumentNullException(nameof(genericType));
            Type[] genericArguments = genericType.GetGenericArguments();
            if (genericArguments.Length != 1 || genericArguments[0].IsGenericType) throw new ArgumentException($"Needs exactly one generic argument. {genericType.FullName} doesn't match criteria for {GetType().FullName}");

            // Create ConditionalWeakTable.CreateValueCallback delegate instance
            cacheCallback = CreateConstructor;
        }

        /// <summary>
        /// Creates new constructor delegate for GenericType{t}. 
        /// Does not use cache.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Func<A0, A1, ReturnType> CreateConstructor(Type t)
        {
            Type runtimeType = genericType.MakeGenericType(t);
            ConstructorInfo ci = runtimeType.GetConstructor(constructorParamTypes);
            if (ci == null) throw new Exception($"{GetType().FullName}: Could not find constructor for {runtimeType.FullName}({String.Join(", ", constructorParamTypes.Select(_ => _.FullName))})");

            // Create delegate
            return (Func<A0, A1, ReturnType>)Expression.Lambda(typeof(Func<A0, A1, ReturnType>), Expression.New(ci, constructorParams), constructorParams).Compile();
        }

        /// <summary>
        /// Get or create constructor delegate for GenericType{t}.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Func<A0, A1, ReturnType> GetOrCreateConstructor(Type t)
            => constructorCache.GetValue(t, cacheCallback);

        /// <summary>
        /// Create new instance of runtime type and arg0.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public ReturnType Create(Type t, A0 arg0, A1 arg1)
            => GetOrCreateConstructor(t)(arg0, arg1);

    }

}
