using LightCore;
using MediatR.Examples;
using NUnit.Framework;
using Shouldly;
using System;
using System.Threading.Tasks;

namespace MediatR.LightCore.Tests
{
    [TestFixture]
    public class BehaviorTests
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
        public async Task PipelineBehaviors()
        {
            var expectedMessageFromHandler = "-- Handling Request";

            var mediator = container.Resolve<IMediator>();
            await mediator.Send(new Ping());
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldContain(expectedMessageFromHandler);
        }

        [Test]
        [Ignore("WIP: Wrong order in test")]
        public async Task OrderedBehaviors_Pong()
        {
            var expectedMessageFromHandler = $"- Starting Up{Environment.NewLine}" +
                $"-- Handling Request{Environment.NewLine}" +
                $"--- Handled Ping: Test_Message{Environment.NewLine}" +
                $"-- Finished Request{Environment.NewLine}" +
                $"- All Done{Environment.NewLine}" +
                $"- All Done with Ping{Environment.NewLine}";

            var mediator = container.Resolve<IMediator>();
            await mediator.Send(new Ping() { Message = "Test_Message" });
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldBe(expectedMessageFromHandler);
        }

        [Test]
        public async Task OrderedBehaviors_Jing()
        {
            var expectedMessageFromHandler = $"- Starting Up{Environment.NewLine}" +
                $"-- Handling Request{Environment.NewLine}" +
                $"--- Handled Jing: Test_Message, no Jong{Environment.NewLine}" +
                $"-- Finished Request{Environment.NewLine}" +
                $"- All Done{Environment.NewLine}";

            var mediator = container.Resolve<IMediator>();
            await mediator.Send(new Jing() { Message = "Test_Message" });
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldBe(expectedMessageFromHandler);
        }
    }
}