using LightCore;
using MediatR.Examples.ExceptionHandler;
using NUnit.Framework;
using Shouldly;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MediatR.LightCore.Tests
{
    [TestFixture]
    public class ExceptionHandlerTests
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
        public async Task HandlerForSameException()
        {
            var expectedMessageFromHandler = $"- Starting Up{Environment.NewLine}" +
                $"-- Handling Request{Environment.NewLine}" +
                $"--- Exception: 'MediatR.Examples.ExceptionHandler.ForbiddenException'{Environment.NewLine}" +
                $"---- Exception Handler: 'MediatR.Examples.ExceptionHandler.AccessDeniedExceptionHandler'{Environment.NewLine}";

            var mediator = container.Resolve<IMediator>();
            await mediator.Send(new PingProtectedResource { Message = "Ping to protected resource" });
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldBe(expectedMessageFromHandler);
        }

        [Test]
        [Ignore("System.Reflection.TargetException : Object does not match target type.")]
        public async Task HandlerForBaseException()
        {
            var expectedMessageFromHandler = $"- Starting Up{Environment.NewLine}" +
                $"-- Handling Request{Environment.NewLine}" +
                $"--- Exception: 'MediatR.Examples.ExceptionHandler.ResourceNotFoundException'{Environment.NewLine}" +
                $"---- Exception Handler: 'MediatR.Examples.ExceptionHandler.ConnectionExceptionHandler'{Environment.NewLine}";

            var mediator = container.Resolve<IMediator>();
            await mediator.Send(new PingResource { Message = "Ping to missed resource" });
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldBe(expectedMessageFromHandler);
        }

        [Test]
        [Description("Current status of framework; should be deleted if main test is running")]
        public void HandlerForBaseException_PartialTest()
        {
            var expectedMessageFromHandler = $"- Starting Up{Environment.NewLine}" +
                $"-- Handling Request{Environment.NewLine}" +
                $"--- Exception: 'MediatR.Examples.ExceptionHandler.ResourceNotFoundException'{Environment.NewLine}";

            var mediator = container.Resolve<IMediator>();
            Should.Throw<TargetException>(() => mediator.Send(new PingResource { Message = "Ping to missed resource" }).GetAwaiter().GetResult());
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldStartWith(expectedMessageFromHandler);
        }

        [Test]
        [Ignore("System.Reflection.TargetException : Object does not match target type.")]
        public async Task HandlerForLessSpecificException()
        {
            var expectedMessageFromHandler = $"- Starting Up{Environment.NewLine}" +
                $"-- Handling Request{Environment.NewLine}" +
                $"--- Exception: 'System.Threading.Tasks.TaskCanceledException'{Environment.NewLine}" +
                $"---- Exception Handler: 'MediatR.Examples.ExceptionHandler.CommonExceptionHandler'{Environment.NewLine}";

            var mediator = container.Resolve<IMediator>();
            await mediator.Send(new PingResourceTimeout { Message = "Ping to ISS resource" });
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldBe(expectedMessageFromHandler);
        }

        [Test]
        [Description("Current status of framework; should be deleted if main test is running")]
        public void HandlerForLessSpecificException_PartialTest()
        {
            var expectedMessageFromHandler = $"- Starting Up{Environment.NewLine}" +
               $"-- Handling Request{Environment.NewLine}" +
               $"--- Exception: 'System.Threading.Tasks.TaskCanceledException'{Environment.NewLine}";

            var mediator = container.Resolve<IMediator>();
            Should.Throw<TargetException>(() => mediator.Send(new PingResourceTimeout { Message = "Ping to ISS resource" }).GetAwaiter().GetResult());
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldStartWith(expectedMessageFromHandler);
        }

        [Test]
        [Ignore("System.Reflection.TargetException : Object does not match target type.")]
        public async Task PreferredHandlerForBaseException()
        {
            var expectedMessageFromHandler = $"- Starting Up{Environment.NewLine}" +
                $"-- Handling Request{Environment.NewLine}" +
                $"--- Exception: 'System.Threading.Tasks.TaskCanceledException'{Environment.NewLine}" +
                $"---- Exception Handler: 'MediatR.Examples.ExceptionHandler.Overrides.CommonExceptionHandler'{Environment.NewLine}";

            var mediator = container.Resolve<IMediator>();
            await mediator.Send(new MediatR.Examples.ExceptionHandler.Overrides.PingResourceTimeout { Message = "Ping to ISS resource (preferred)" });
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldBe(expectedMessageFromHandler);
        }

        [Test]
        [Description("Current status of framework; should be deleted if main test is running")]
        public void PreferredHandlerForBaseException_PartialTest()
        {
            var expectedMessageFromHandler = $"- Starting Up{Environment.NewLine}" +
               $"-- Handling Request{Environment.NewLine}" +
               $"--- Exception: 'System.Threading.Tasks.TaskCanceledException'{Environment.NewLine}";

            var mediator = container.Resolve<IMediator>();
            Should.Throw<TargetException>(() => mediator.Send(new MediatR.Examples.ExceptionHandler.Overrides.PingResourceTimeout { Message = "Ping to ISS resource (preferred)" }).GetAwaiter().GetResult());
            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldStartWith(expectedMessageFromHandler);
        }

        [Test]
        public async Task OverriddenHandlerForBaseException()
        {
            var expectedMessageFromHandler = $"- Starting Up{Environment.NewLine}" +
                $"-- Handling Request{Environment.NewLine}" +
                $"--- Exception: 'MediatR.Examples.ExceptionHandler.ServerException'{Environment.NewLine}" +
                $"---- Exception Handler: 'MediatR.Examples.ExceptionHandler.Overrides.ServerExceptionHandler'{Environment.NewLine}";

            var mediator = container.Resolve<IMediator>();
            await mediator.Send(new PingNewResource { Message = "Ping to ISS resource (override)" });

            string actualMessageForNotification = writer.Contents;

            actualMessageForNotification.ShouldBe(expectedMessageFromHandler);
        }
    }
}