using LightCore;
using MediatR.Examples;
using NUnit.Framework;
using Shouldly;
using System.Threading.Tasks;

namespace MediatR.LightCore.Tests
{
    [TestFixture]
    internal class ProcessorTests
    {
        private IContainer container;
        private StringWriter writer;

        [SetUp]
        public void SetUp()
        {
            this.writer = new StringWriter();
            var builder = new ContainerBuilder();
            builder.RegisterModule(new MediatRModule(typeof(NotificationTests).Assembly));

            builder.Register<System.IO.TextWriter>(ctn => writer);
            this.container = builder.Build();
        }

        [Test]
        public async Task PreProcessor()
        {
            var expectedMessageFromHandler = "- Starting Up";

            var mediator = container.Resolve<IMediator>();
            await mediator.Send(new Ping());
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldContain(expectedMessageFromHandler);
        }

        [Test]
        public async Task PostProcessor()
        {
            var expectedMessageFromHandler = "- All Done";

            var mediator = container.Resolve<IMediator>();
            await mediator.Send(new Ping());
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldContain(expectedMessageFromHandler);
        }

        [Test]
        public async Task ConstrainedPostProcessor()
        {
            var expectedMessageFromHandler = "- All Done with Ping";

            var mediator = container.Resolve<IMediator>();
            await mediator.Send(new Ping());
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldContain(expectedMessageFromHandler);
        }
    }
}