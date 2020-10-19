using LightCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatR.LightCore
{
    static partial class Register
    {
        internal static void IPipelineBehavior(IContainerBuilder containerBuilder, IEnumerable<Type> types, IEnumerable<Type[]> genericArguments)
        {
            var pipelineBehaviorType = typeof(IPipelineBehavior<,>);
            InnerRegister(containerBuilder, types, pipelineBehaviorType, genericArguments);
        }
    }
}
