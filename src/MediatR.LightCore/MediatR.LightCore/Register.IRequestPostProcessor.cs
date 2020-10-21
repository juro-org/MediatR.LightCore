using LightCore;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;

namespace MediatR.LightCore
{
    static partial class Register
    {
        internal static void IRequestPostProcessor(IContainerBuilder containerBuilder, IEnumerable<Type> types, IEnumerable<Type[]> genericArguments)
        {
            var pipelineBehaviorType = typeof(IRequestPostProcessor<,>);
            var registerCount = InnerRegister(containerBuilder, types, pipelineBehaviorType, genericArguments);
            if (registerCount > 0)
            {
                InnerRegister(containerBuilder, new Type[] { typeof(RequestPostProcessorBehavior<,>) }, typeof(IPipelineBehavior<,>), genericArguments);
            }
        }
    }
}