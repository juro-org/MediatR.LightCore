using LightCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediatR.LightCore
{
    internal static partial class Register
    {
        private static bool IsConcrete(this Type type)
        {
            return !type.IsAbstract && type.IsClass;
        }

        private static bool IsMatchingInterface(Type possibleType, Type expectedInterface)
        {
            if (possibleType == null || expectedInterface == null)
            {
                return false;
            }

            if (possibleType.IsInterface)
            {
                return possibleType.GetGenericTypeDefinition().Equals(expectedInterface);
            }

            return IsMatchingInterface(possibleType.GetInterface(expectedInterface.Name), expectedInterface);
        }

        private static bool IsTypeWithGenericType(Type possibleType, Type expected)
        {
            return IsConcrete(possibleType) && IsMatchingInterface(possibleType, expected) && !possibleType.IsGenericType;
        }

        private static Type GetInterfaceImpl(Type possibleType, Type expected)
        {
            return possibleType.GetInterface(expected.Name);
        }

        private static bool IsTypeWithoutGenericType(Type possibleType, Type expected)
        {
            return IsConcrete(possibleType) && IsMatchingInterface(possibleType, expected) && possibleType.IsGenericType;
        }

        private static int InnerRegister(IContainerBuilder containerBuilder, IEnumerable<Type> types, Type typeToRegister, IEnumerable<Type[]> genericArguments)
        {
            var count = 0;
            types.Where(t => IsTypeWithGenericType(t, typeToRegister))
                .ToList()
                 .ForEach(type =>
                 {
                     var regType = GetInterfaceImpl(type, typeToRegister);
                     var typeGenericArguments = regType.GetGenericArguments();
                     if (typeGenericArguments.Any(x => IsConcrete(x)))
                     {
                         containerBuilder.Register(regType, type);
                         count++;
                         return;
                     }

                     count = RegisterForAllGenericArguments(containerBuilder, typeToRegister, genericArguments, type, count);
                     count++;
                 });
            types.Where(t => IsTypeWithoutGenericType(t, typeToRegister))
               .ToList()
                .ForEach(type =>
                {
                    count = RegisterForAllGenericArguments(containerBuilder, typeToRegister, genericArguments, type, count);
                });
            return count;
        }

        private static int RegisterForAllGenericArguments(IContainerBuilder containerBuilder, Type typeToRegister, IEnumerable<Type[]> genericArguments, Type type, int count)
        {
            genericArguments.ToList().ForEach(genericArgument =>
            {
                var regType = typeToRegister.MakeGenericType(genericArgument);
                var typeGenericArguments = type.GetGenericArguments();
                bool isAssignable = true;
                for (int i = 0; i < typeGenericArguments.Length; i++)
                {
                    isAssignable &= typeGenericArguments[i].BaseType.IsAssignableFrom(genericArgument[i]);
                }
                if (!isAssignable) { return; }
                var regClass = type;
                if (type.IsGenericType)
                {
                    regClass = type.MakeGenericType(genericArgument);
                }
                containerBuilder.Register(regType, regClass);
                count++;
            });
            return count;
        }
    }
}