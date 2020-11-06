using LightCore.Registration;
using MediatR.Pipeline;
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

            containerBuilder.Register(ctn => new ServiceFactory(type => LightCoreServiceFactory(ctn, type))).ControlledBy<LC.Lifecycle.SingletonLifecycle>();
            containerBuilder.Register<IMediator>(ctn => new Mediator(ctn.Resolve));

            IEnumerable<Type[]> genericArguments;

            MediatR.LightCore.Register.IRequestHandler(containerBuilder, allTypesOfAssemblies, out genericArguments);
            MediatR.LightCore.Register.IRequestExceptionAction(containerBuilder, allTypesOfAssemblies, genericArguments);
            MediatR.LightCore.Register.IRequestPreProcessor(containerBuilder, allTypesOfAssemblies, genericArguments);
            MediatR.LightCore.Register.IRequestPostProcessor(containerBuilder, allTypesOfAssemblies, genericArguments);
            //Register IPipelineBehavior after Pre- and PostProcessor for correct order.
            MediatR.LightCore.Register.IPipelineBehavior(containerBuilder, allTypesOfAssemblies, genericArguments);
            MediatR.LightCore.Register.INotificationHandler(containerBuilder, allTypesOfAssemblies);
        }

        private static Func<LC.IContainer, Type, object> LightCoreServiceFactory = (ctn, type) =>
        {
            if (type.IsInterface && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var generichArgumentType = type.GetGenericArguments()[0];
                var exceptionTypesToResolve = new Type[]
                {
                    typeof(IRequestExceptionAction<,>),
                    typeof(IRequestExceptionHandler<,,>)
                };
                foreach (var typeToResolve in exceptionTypesToResolve)
                {
                    if (generichArgumentType.IsInterface && generichArgumentType.GetGenericTypeDefinition() == typeToResolve)
                    {
                        return Resolve(ctn, typeToResolve, generichArgumentType.GetGenericArguments());
                    }
                }
            }

            return ctn.Resolve(type);
        };

        private static IEnumerable<object> Resolve(LC.IContainer ctn, Type interfaceType, Type[] genericArgumentTypes, int idx = 0)
        {
            var result = new List<object>();

            if (genericArgumentTypes.Length <= idx)
            {
                var typeToCheck = interfaceType.MakeGenericType(genericArgumentTypes);
                if (ctn.HasRegistration(typeToCheck))
                {
                    return ctn.ResolveAll(typeToCheck);
                }
                return result;
            }

            var argumentType = genericArgumentTypes[idx];
            do
            {
                if (interfaceType.GetGenericArguments()[idx].BaseType.IsAssignableFrom(argumentType))
                {
                    int counter = 0;

                    result.AddRange(Resolve(ctn,
                        interfaceType,
                        genericArgumentTypes.ToList().Select(t =>
                        {
                            if (counter == idx)
                            {
                                counter++;
                                return argumentType;
                            }
                            counter++;
                            return t;
                        }).ToArray(),
                    (idx + 1)));
                }
                argumentType = argumentType.BaseType;
            } while (argumentType != null);
            return result;
        }
    }
}