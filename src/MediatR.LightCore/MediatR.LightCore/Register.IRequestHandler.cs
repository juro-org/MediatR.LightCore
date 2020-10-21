using LightCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediatR.LightCore
{
    internal static partial class Register
    {
        internal static void IRequestHandler(IContainerBuilder containerBuilder, IEnumerable<Type> types, out IEnumerable<Type[]> genericArguments)
        {
            var requestHandlerType = typeof(IRequestHandler<,>);
            genericArguments = types.Where(t => IsTypeWithGenericType(t, requestHandlerType))
                .Select(type =>
                {
                    var interfaceType = GetInterfaceImpl(type, requestHandlerType);
                    containerBuilder.Register(interfaceType, type);
                    return interfaceType.GetGenericArguments();
                }
                ).ToList();
        }
    }
}