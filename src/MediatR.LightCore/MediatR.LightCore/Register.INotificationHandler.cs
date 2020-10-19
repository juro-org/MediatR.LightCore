using LightCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediatR.LightCore
{
    partial class Register
    {
        internal static void INotificationHandler(IContainerBuilder containerBuilder, IEnumerable<Type> types)
        {
            var iNotifications = types.Where(p => p.IsClass && !p.IsAbstract && p.GetInterfaces().Any(t => t.FullName != null && t.FullName.Equals(typeof(INotification).FullName))).Select(t => new[] { t });
            var iNotificationHandlerType = typeof(INotificationHandler<>);
            InnerRegister(containerBuilder, types, iNotificationHandlerType, iNotifications);
        }
    }
}
