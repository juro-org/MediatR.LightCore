using LightCore;
using LightCore.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public override void Register(IContainerBuilder containerBuilder)
        {
            var allTypesOfAssemblies = mediatorAssemblies.SelectMany(a => a.GetTypes());

            containerBuilder.Register<IMediator>(ctn => new Mediator(ctn.Resolve));

            IEnumerable<Type[]> genericArguments;
            MediatR.LightCore.Register.IRequestHandler(containerBuilder, allTypesOfAssemblies, out genericArguments);
            MediatR.LightCore.Register.IPipelineBehavior(containerBuilder, allTypesOfAssemblies, genericArguments);
            MediatR.LightCore.Register.INotificationHandler(containerBuilder, allTypesOfAssemblies);
        }
    }
}
