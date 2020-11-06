using LightCore;
using MediatR.Examples;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MediatR.LightCore.Tests
{
    [TestFixture]
    public class NotificationTests
    {
        private IContainer container;
        private StringWriter writer;

        [SetUp]
        public void SetUp()
        {
            this.writer = new StringWriter();
            var builder = new ContainerBuilder();
            builder.RegisterModule(new MediatRModule(typeof(NotificationTests).Assembly));
            //builder.Register<INotificationHandler<Pinged>, GenericHandler>();
            builder.Register<System.IO.TextWriter>(ctn => writer);
            this.container = builder.Build();
        }

        [Test]
        public async Task NotificationHandler()
        {
            var expectedMessageFromHandler = "Got pinged async.";

            var mediator = container.Resolve<IMediator>();
            await mediator.Publish(new Pinged());
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldContain(expectedMessageFromHandler);
        }

        [Test]
        public async Task AlsoNotificationHandler()
        {
            var expectedMessageFromHandler = "Got pinged also async.";

            var mediator = container.Resolve<IMediator>();
            await mediator.Publish(new Pinged());
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldContain(expectedMessageFromHandler);
        }

        [Test]
        public async Task ConstrainedNotificationHandler()
        {
            var handler = typeof(INotificationHandler<>);
            var concretHandler = typeof(GenericHandler);

            var intf = concretHandler.GetInterface(handler.Name);
            var a = concretHandler.GenericTypeArguments.SequenceEqual(handler.GenericTypeArguments);
            var expectedMessageFromHandler = "Got notified";

            var mediator = container.Resolve<IMediator>();
            await mediator.Publish(new Pinged());
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldContain(expectedMessageFromHandler);
        }

        [Test]
        public async Task CovariantNotificationHandler()
        {
            var expectedMessageFromHandler = "Got pinged constrained async.";

            var mediator = container.Resolve<IMediator>();
            await mediator.Publish(new Pinged());
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldContain(expectedMessageFromHandler);
        }

        [Test]
        public async Task MultipleNotificationHandlers()
        {
            var expectedMessageFromHandlers = $"Got notified.{Environment.NewLine}" +
                $"Got pinged async.{Environment.NewLine}" +
                $"Got pinged also async.{Environment.NewLine}" +
                $"Got pinged constrained async.{Environment.NewLine}";

            var mediator = container.Resolve<IMediator>();
            await mediator.Publish(new Pinged());
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldBe(expectedMessageFromHandlers);
        }
    }
}