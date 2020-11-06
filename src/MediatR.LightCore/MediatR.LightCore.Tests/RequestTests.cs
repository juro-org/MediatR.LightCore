using LightCore;
using MediatR.Examples;
using NUnit.Framework;
using Shouldly;
using System.Threading.Tasks;

namespace MediatR.LightCore.Tests
{
    [TestFixture]
    public class RequestTests
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
        public async Task RequestHandler()
        {
            var expectedMessageFromHandler = "--- Handled Ping:";

            var mediator = container.Resolve<IMediator>();
            await mediator.Send(new Ping());
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldContain(expectedMessageFromHandler);
        }

        [Test]
        public async Task VoidRequestsHandler()
        {
            var expectedMessageFromHandler = "--- Handled Jing:";

            var mediator = container.Resolve<IMediator>();
            await mediator.Send(new Jing());
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldContain(expectedMessageFromHandler);
        }
    }
}