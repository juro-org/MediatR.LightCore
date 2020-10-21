using LightCore.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LC = LightCore;

namespace MediatR.LightCore
{
    public class MediatRModule : RegistrationModule
    {
        /// <summary>
        /// The assemblies from the MediatR implementation types.
        /// </summary>
        private readonly List<Assembly> mediatorAssemblies;

        /// <summary>
        /// Initializes a new instance of <see cref="MediatRModule" />.
        /// </summary>
        protected MediatRModule()
        {
            mediatorAssemblies = new List<Assembly>();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MediatRModule" />.
        /// </summary>
        /// <param name="assembly">The mediatR types assembly.</param>
        public MediatRModule(Assembly assembly)
            : this(new[] { assembly })
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MediatRModule" />.
        /// </summary>
        /// <param name="assemblies">The mediatR types  assemblies.</param>
        public MediatRModule(params Assembly[] assemblies)
            : this()
        {
            mediatorAssemblies.AddRange(assemblies);
        }

        public override void Register(LC.IContainerBuilder containerBuilder)
        {
            var allTypesOfAssemblies = mediatorAssemblies.SelectMany(a => a.GetTypes());

            containerBuilder.Register(ctn => new ServiceFactory(ctn.Resolve)).ControlledBy<LC.Lifecycle.SingletonLifecycle>();
            containerBuilder.Register<IMediator>(ctn => new Mediator(ctn.Resolve));

            IEnumerable<Type[]> genericArguments;
            MediatR.LightCore.Register.IRequestHandler(containerBuilder, allTypesOfAssemblies, out genericArguments);
            MediatR.LightCore.Register.IRequestPreProcessor(containerBuilder, allTypesOfAssemblies, genericArguments);
            MediatR.LightCore.Register.IRequestPostProcessor(containerBuilder, allTypesOfAssemblies, genericArguments);
            //Register IPipelineBehavior after Pre- and PostProcessor for correct order.
            MediatR.LightCore.Register.IPipelineBehavior(containerBuilder, allTypesOfAssemblies, genericArguments);
            MediatR.LightCore.Register.INotificationHandler(containerBuilder, allTypesOfAssemblies);
        }
    }
}