using LightCore;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediatR.LightCore
{
    static partial class Register
    {
        internal static void IRequestExceptionAction(IContainerBuilder containerBuilder, IEnumerable<Type> types, IEnumerable<Type[]> genericArguments)
        {
            var typeToRegister = typeof(IRequestExceptionAction<,>);
            InnerRegister(containerBuilder, types, typeToRegister, genericArguments.Select(t => new Type[] { t[0], typeof(Exception) }));//Only TRequest

            typeToRegister = typeof(IRequestExceptionHandler<,,>);
            InnerRegister(containerBuilder, types, typeToRegister, genericArguments.Select(t => new Type[] { t[0], t[1], typeof(Exception) }));//Only TRequest

            InnerRegister(containerBuilder, new Type[] {  typeof(RequestExceptionProcessorBehavior<,>),
                typeof(RequestExceptionActionProcessorBehavior<,>) }, typeof(IPipelineBehavior<,>), genericArguments);
        }
    }
}