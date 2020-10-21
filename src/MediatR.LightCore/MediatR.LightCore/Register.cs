using LightCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediatR.LightCore
{
    internal static partial class Register
    {
        private static bool IsTypeWithGenericType(Type possibleType, Type expected)
        {
            return possibleType.IsClass && !possibleType.IsAbstract && possibleType.GetInterfaces().Any(t => t.FullName != null && t.FullName.StartsWith(expected.FullName));
        }

        private static Type GetInterfaceImpl(Type possibleType, Type expected)
        {
            return possibleType.GetInterfaces().First(t => t.FullName != null && t.FullName.StartsWith(expected.FullName));
        }

        private static bool IsTypeWithoutGenericType(Type possibleType, Type expected)
        {
            return possibleType.IsClass && !possibleType.IsAbstract && possibleType.GetInterfaces().Any(t => t.FullName == null && t.Namespace.Equals(expected.Namespace) && t.Name.Equals(expected.Name));
        }

        private static int InnerRegister(IContainerBuilder containerBuilder, IEnumerable<Type> types, Type typeToRegister, IEnumerable<Type[]> genericArguments)
        {
            var count = 0;
            types.Where(t => IsTypeWithGenericType(t, typeToRegister))
                .ToList()
                 .ForEach(type =>
                 {
                     var regType = GetInterfaceImpl(type, typeToRegister);
                     containerBuilder.Register(regType, type);
                     count++;
                 });
            types.Where(t => IsTypeWithoutGenericType(t, typeToRegister))
               .ToList()
                .ForEach(type =>
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
                        var regClass = type.MakeGenericType(genericArgument);
                        containerBuilder.Register(regType, regClass);
                        count++;
                    });
                });
            return count;
        }
    }
}