using LightCore;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediatR.LightCore
{
    static partial class Register
    {
        internal static void IRequestPreProcessor(IContainerBuilder containerBuilder, IEnumerable<Type> types, IEnumerable<Type[]> genericArguments)
        {
            var typeToRegister = typeof(IRequestPreProcessor<>);
            var registerCount = InnerRegister(containerBuilder, types, typeToRegister, genericArguments.Select(t => new Type[] { t[0] }));//Only TRequest
            if (registerCount > 0)
            {
                InnerRegister(containerBuilder, new Type[] { typeof(RequestPreProcessorBehavior<,>) }, typeof(IPipelineBehavior<,>), genericArguments);
            }
        }
    }
}