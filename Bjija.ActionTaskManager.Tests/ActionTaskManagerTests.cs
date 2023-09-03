using Bjija.ActionTaskManager.Helpers;
using Bjija.ActionTaskManager.Models;
using NSubstitute;

namespace Bjija.ActionTaskManager.Tests
{
    public class ActionTaskManagerTests
    {
        [Fact]
        public async Task TestTaskExecutionOnAction()
        {
            // Arrange
            var mockTask = Substitute.For<ITask<Payload>>();
            var mockTaskFactory = Substitute.For<ITaskFactory>();
            var mockPipelineFactory = Substitute.For<ITaskPipelineFactory>();
            var taskManagerOptions = new ActionTaskManagerOptions();

            // Configure task factory to return the mock task when a specific task type is requested
            mockTaskFactory.Create<ITask<Payload>>(typeof(TestExampleTask)).Returns(mockTask);

            var taskManager = new ActionTaskManager(taskManagerOptions, mockTaskFactory, mockPipelineFactory);

            // Register task to action
            taskManager.Register<UserLoginAction, Payload, TestExampleTask>();

            var userLoginAction = new UserLoginAction();

            // Act
            await taskManager.LinkActionToTasksAsync(userLoginAction);

            // Add debug logs
            userLoginAction.ActionOccurred += (args) => {
                Console.WriteLine("Event fired");
            };
            mockTask.When(x => x.ExecuteAsync(Arg.Any<ActionEventArgs<Payload>>()))
                    .Do(x => {
                        Console.WriteLine("ExecuteAsync called");
                    });

            userLoginAction.UserLoggedIn(new Payload());

            // Assert
            await mockTask.Received(1).ExecuteAsync(Arg.Any<ActionEventArgs<Payload>>());
        }
        [Fact]
        public async Task UserLoginAction_ShouldTrigger_WelcomeEmailTask()
        {
            // Arrange
            var mockTask = Substitute.For<ITask<Payload>>();
            var mockTaskFactory = Substitute.For<ITaskFactory>();
            var mockPipelineFactory = Substitute.For<ITaskPipelineFactory>();
            var taskManagerOptions = new ActionTaskManagerOptions();
            mockTaskFactory.Create<ITask<Payload>>(typeof(TestExampleTask)).Returns(mockTask);

            var taskManager = new ActionTaskManager(taskManagerOptions, mockTaskFactory, mockPipelineFactory);
            taskManager.Register<UserLoginAction, Payload, TestExampleTask>();

            var userLoginAction = new UserLoginAction();
            await taskManager.LinkActionToTasksAsync(userLoginAction);

            // Act
            userLoginAction.UserLoggedIn(new Payload());

            // Assert
            await mockTask.Received(1).ExecuteAsync(Arg.Any<ActionEventArgs<Payload>>());
        }

        [Fact]
        public async Task UserProfileChoiceAction_ShouldTrigger_ComptableTasks()
        {
            // Arrange
            var mockTask1 = Substitute.For<ITask<Payload>>();
            var mockTask2 = Substitute.For<ITask<Payload>>();
            var mockTaskFactory = Substitute.For<ITaskFactory>();
            var mockPipelineFactory = Substitute.For<ITaskPipelineFactory>();
            var taskManagerOptions = new ActionTaskManagerOptions();
            mockTaskFactory.Create<ITask<Payload>>(typeof(TestExampleTask)).Returns(mockTask1);
            mockTaskFactory.Create<ITask<Payload>>(typeof(TestExampleTask2)).Returns(mockTask2);

            var taskManager = new ActionTaskManager(taskManagerOptions, mockTaskFactory, mockPipelineFactory);
            taskManager.RegisterProfileTask("Comptable-flow", mockTask1);
            taskManager.RegisterProfileTask("Comptable-flow", mockTask2);

            var userProfileAction = new UserProfileChoiceAction(new Payload());
            await taskManager.LinkActionToTasksAsync(userProfileAction, "Comptable-flow");

            // Act
            await userProfileAction.ChooseProfile("Comptable-flow");

            // Assert
            await mockTask1.Received(1).ExecuteAsync(Arg.Any<ActionEventArgs<Payload>>());
            await mockTask2.Received(1).ExecuteAsync(Arg.Any<ActionEventArgs<Payload>>());
        }

    }

    public class UserLoginAction : IActionTrigger<Payload>
    {
        public string ProfileName => null;// or "";

        public Action<ActionEventArgs<Payload>> ActionOccurred { get; set; }

        public void UserLoggedIn(Payload payload)
        {
            ActionOccurred?.Invoke(new ActionEventArgs<Payload>(payload));

        }
    }

    public class UserProfileChoiceAction : IActionTrigger<Payload>
    {
        private readonly Payload _payload;

        public UserProfileChoiceAction(Payload payload)
        {
            _payload = payload;
        }

        public Action<ActionEventArgs<Payload>> ActionOccurred { get; set; }

        public string ProfileName { get; private set; }

        public async Task ChooseProfile(string profileData)
        {
            ActionOccurred?.Invoke(new ActionEventArgs<Payload>(_payload));
        }
    }

    public class Payload
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime BirthDay { get; set; }
        public double Salary { get; set; }
    }

    public class TestExampleTask : Task<Payload>
    {
        protected override async Task ExecuteCoreAsync(ActionEventArgs<Payload> args)
        {
            string username = args.Data.Email;
            await SendEmailAsync(username);
        }

        private Task SendEmailAsync(string username)
        {
            // Mocked email sending method
            Console.WriteLine($"Sending email task to {username}");
            return Task.CompletedTask;
        }
    }

    public class TestExampleTask2 : Task<Payload>
    {
        protected override async Task ExecuteCoreAsync(ActionEventArgs<Payload> args)
        {
            string username = args.Data.Email;
            await SendEmailAsync(username);
        }

        private Task SendEmailAsync(string username)
        {
            // Mocked email sending method
            Console.WriteLine($"Sending email task to {username}");
            return Task.CompletedTask;
        }
    }
}