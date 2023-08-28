# Bijia Action Task Manager

Bijia Action Task Manager is a flexible and extensible .NET library designed to streamline the task execution pipeline based on various action triggers. It provides a clean abstraction for associating tasks with actions, allowing developers to automate workflows and perform asynchronous operations seamlessly.

## Key Features

- **Action-Based Task Execution**: Execute one or more tasks when a specific action occurs in your application.
- **Profile-Based Task Management**: Run multiple tasks sequentially or in parallel for specific user profiles.
- **Extensible Decorators**: Easily add pre-execution or post-execution behavior to tasks.
- **Dynamic Task Pipelines**: Create, modify, and manage task pipelines at runtime.
- **Logging and Monitoring**: Built-in support for task logging and event-based monitoring.

Ideal for applications that require modular and configurable task automation based on various events or triggers.

## Getting Started

### Installation

### Basic Registration
To get started, you need to register the Bijia Action Task Manager in your application's dependency injection container.
Register the Bijia Action Task Manager in the `Startup.cs` or equivalent configuration file:

```csharp
var services = new ServiceCollection();
services.AddBijiaActionTaskManager();
```

### Enabling Logging Decorators

You can enable logging decorators for all tasks by setting the `EnableLoggingDecorator` option to `true`.

```csharp
services.AddBijiaActionTaskManager(options =>
{
    options.EnableLoggingDecorator = true;
});
```

### Logging Configuration

The library integrates with Microsoft's `ILogger` abstraction and uses the console logger by default.

#### Using Serilog

If you have Serilog configured in your application, the library will automatically use it:

```csharp
services.AddLogging(builder =>
{
    builder.AddSerilog();
});
```

#### Custom Logging Configuration

If you want to plug in any logger that implements `ILogger`, you can set the `LoggerFactory` option:

```csharp
services.AddBijiaActionTaskManager(options =>
{
    options.EnableLoggingDecorator = false;
    options.LoggerFactory = LoggerFactory.Create(builder => { builder.AddSerilog(); });
});
```
## Bijia Action Task Manager Examples

### Basics: Implementing Interfaces and Base Classes

#### Payload Definition

Define a payload class to pass data to your tasks. Here's an example:

```csharp
Payload payload = new Payload
{
    BirthDay = new DateTime(1985, 09, 24),
    Email = "no-reply@gmail.com",
    Name = "Bouchalkha",
};
```
#### Task Definition

Tasks must inherit from the base class `Task<Payload>` and override the `ExecuteCoreAsync` method. Here's an example:

```csharp
public class WelcomeEmailTask : Bjija.ActionTaskManager.Task<Payload>
{
    protected override async Task ExecuteCoreAsync(object sender, ActionEventArgs<Payload> args)
    {
        string username = args.Data.Email;
        await SendEmailAsync(username);
    }

    private Task SendEmailAsync(string username)
    {
        Console.WriteLine($"Sending email task to {username}");
        return Task.CompletedTask;
    }
}
```

#### Action Definition

Actions must implement the `IActionTrigger<Payload>` interface. Here's an example for user login:

```csharp
public class UserLoginAction : IActionTrigger<Payload>
{
    public string ProfileName => null;
    public event EventHandler<ActionEventArgs<Payload>> ActionOccurred;

    public void UserLoggedIn(Payload payload)
    {
        ActionOccurred?.Invoke(this, new ActionEventArgs<Payload>(payload));
    }
}
```
#### Registering Universal Decorators
Universal decorators are decorators that are applied to all tasks, regardless of the action they are associated with. This is useful when you want to apply common behavior, like logging, to all tasks. You can register a universal decorator using the `AddUniversalDecorator` method.

```csharp
taskManager.AddUniversalDecorator(typeof(LoggingTaskDecorator<string>));
```
By using this method, all tasks will be decorated with LoggingTaskDecorator<string>, enabling consistent logging behavior across tasks.

### Example 1: Registering Tasks for Specific Actions

#### Task Registration

To register tasks for a specific action, you can use the `Register` method provided by `IActionTaskManager`. Here's how to register the `WelcomeEmailTask` and `SetupUserSettings` for the `UserLoginAction`:

```csharp
taskManager.Register<UserLoginAction, Payload, SetupUserSettings>()
           .Register<UserLoginAction, Payload, WelcomeEmailTask>(); 

```
##### with a specific decorator like a logging for example

```csharp
taskManager.Register<UserLoginAction, Payload, SetupUserSettings>(typeof(AnotherDecorator<Payload>))
           .Register<UserLoginAction, Payload, WelcomeEmailTask>(typeof(LoggingTaskDecorator<Payload>)); 

```
##### Conditional Task Execution

Sometimes you may want to conditionally execute tasks based on certain criteria. You can achieve this by providing a predicate function during task registration.

#### Creating a Predicate Function

Create a predicate function that takes `ActionEventArgs<Payload>` and returns a `bool`. The task will only execute if the function returns `true`.

```csharp
Func<ActionEventArgs<Payload>, bool> predicate = args => args.Data.Email != "no-reply@gmail.com";
```
Now, the `WelcomeEmailTask` will only execute if the user's email is not "no-reply@gmail.com".

#### Registering the Task with the Predicate

Register the task and provide the predicate function as a parameter.

```csharp
taskManager.Register<UserLoginAction, Payload, WelcomeEmailTask>(predicate);
```
Or predicate with a task decorator

```csharp
taskManager.Register<UserLoginAction, Payload, WelcomeEmailTask>(args => args.Data.Email != "no-reply@gmail.com", typeof(AnotherDecorator<Payload>));
```

#### Linking Actions to Tasks

After registering tasks, you need to link them to the corresponding actions using the `LinkActionToTasksAsync` method:

```csharp
await taskManager.LinkActionToTasksAsync(userLoginAction);
```

#### Triggering Actions

Finally, to trigger the action and consequently execute the associated tasks, you can call a method on the action object that fires the action. In our `UserLoginAction` example, this would be the `UserLoggedIn` method:

```csharp
userLoginAction.UserLoggedIn(payload);
```
By calling this method, all tasks registered for this action will be executed in the order they were registered.

```csharp
  var userLoginAction = new UserLoginAction();

  taskManager.Register<UserLoginAction, Payload, WelcomeEmailTask>(args => args.Data.Email != "no-reply@gmail.com")
             .Register<UserLoginAction, Payload, SomeOtherTask>(typeof(LoggingTaskDecorator<Payload>));

  taskManager.LinkActionToTasksAsync(userLoginAction);

  userLoginAction.UserLoggedIn(payload);
```

### Example 2: Registering Tasks for Specific Profiles (Regroup tasks by string value)

#### Profile Task Registration

For profile-based tasks, use the `RegisterProfileTask` method provided by `IActionTaskManager`. Below is an example of registering the `SetupUserSettingsTask` and `SendUserWelcomeEmailTask` for the profile "UserLogin":

```csharp
taskManager.RegisterProfileTask("UserLogin", new SetupUserSettingsTask())
           .Register(new SendUserWelcomeEmailTask(), typeof(LoggingTaskDecorator<Payload>));
```

#### Conditional Task Execution for Profiles

As with action-based tasks, you can also conditionally execute profile-based tasks.

```csharp
Func<ActionEventArgs<Payload>, bool> predicate = args => args.Data.Email != string.Empty;
```

You can register a profile-based task with a predicate like this:

```csharp
taskManager.RegisterProfileTask("UserLogin", new SetupUserSettingsTask(), predicate);
```

#### Linking Profiles to Tasks

After registering the profile-based tasks, you can link them to the corresponding profiles using the `LinkActionToTasksAsync` method:

```csharp
await taskManager.LinkActionToTasksAsync(userProfileAction, "UserLogin");
```

#### Triggering Profile-Based Actions

To trigger the profile-based actions, you can call a method on the action object that fires the action and pass in the profile name. In our example, this would be the `ChooseProfile` method on the `UserProfileChoiceAction`:

```csharp
userProfileAction.ChooseProfile("UserLogin");
```
By doing so, all tasks registered for this profile will be executed in the order they were registered.
```csharp
var userProfileAction = new UserProfileChoiceAction(payload);

taskManager.RegisterProfileTask("UserLogin", new SetupUserSettingsTask())
           .Register(new SendUserWelcomeEmailTask(), typeof(LoggingTaskDecorator<Payload>));

await taskManager.LinkActionToTasksAsync(userProfileAction, "UserLogin");

userProfileAction.ChooseProfile("UserLogin");
```

### Example 3: Using Task Pipelines with Events

#### Creating a Task Pipeline

For more complex scenarios, you can create a task pipeline using the `ITaskPipeline<T>` interface:

```csharp
var eCommercePipeline = serviceProvider.GetRequiredService<ITaskPipeline<OrderData>>();
```
#### Registering Tasks in the Pipeline

The concept of a task pipeline in Bijia Action Task Manager is analogous to the middleware pipeline in ASP.NET Core. Just like middleware, tasks in a pipeline are executed in a specific order determined by their priority. They all share and consume the same payload, allowing one task to prepare data or set states that the following task can then use.
In the task pipeline, all tasks consume the same payload. For example, in a pipeline handling `OrderData`, the `ValidateCustomer` task might validate the customer details and mark the order as validated. The next task, `AdjustInventory`, can then use this validated flag to proceed with its own operations.

Tasks can be registered in the pipeline with priority and an optional predicate for conditional execution.

```csharp
eCommercePipeline.RegisterTask(new ValidateCustomer(), priority: 1, predicate: args => args.Data.CustomerId != string.Empty);
eCommercePipeline.RegisterTask(new AdjustInventory(), priority: 2);
eCommercePipeline.RegisterTask(new UpdateCustomer(), priority: 3);
eCommercePipeline.RegisterTask(new ProcessBilling(), priority: 4);
eCommercePipeline.RegisterTask(new SendNotification(), priority: 5);
```
#### Registering the Pipeline with an Action

Once your pipeline is set up, you need to register it with an action:

```csharp
taskManager.RegisterPipeline<CommerceManager, OrderData>(eCommercePipeline);
```
#### Pipeline Events

You can subscribe to various pipeline events to monitor its status:

```csharp
eCommercePipeline.PipelineStarted += (sender, args) => Console.WriteLine("Pipeline started");
eCommercePipeline.PipelineCompleted += (sender, args) => Console.WriteLine("Pipeline completed");
eCommercePipeline.PipelineFailed += (sender, args) => Console.WriteLine($"Pipeline failed: {args.Error}");
```

#### Linking Actions to Pipelines
The pipeline can be linked to actions in the same way as individual tasks:

```csharp
await taskManager.LinkActionToTasksAsync(commerceManager);
```
#### Triggering Actions with Pipelines

Triggering the action will execute the tasks in the pipeline in the order set by their priorities:

```csharp
var commerceManager = new CommerceManager();
commerceManager.ProcessOrder(orderData);
```
#### Here's how it looks in a full example:

```csharp
var commerceManager = new CommerceManager();

var eCommercePipeline = serviceProvider.GetRequiredService<ITaskPipeline<OrderData>>();
eCommercePipeline.RegisterTask(new ValidateCustomer(), priority: 1, predicate: args => args.Data.CustomerId != string.Empty);
eCommercePipeline.RegisterTask(new AdjustInventory(), priority: 2);
eCommercePipeline.RegisterTask(new UpdateCustomer(), priority: 3);
eCommercePipeline.RegisterTask(new ProcessBilling(), priority: 4);
eCommercePipeline.RegisterTask(new SendNotification(), priority: 5);

taskManager.RegisterPipeline<CommerceManager, OrderData>(eCommercePipeline);

eCommercePipeline.PipelineStarted += (sender, args) => Console.WriteLine("Pipeline started");
eCommercePipeline.PipelineCompleted += (sender, args) => Console.WriteLine("Pipeline completed");
eCommercePipeline.PipelineFailed += (sender, args) => Console.WriteLine($"Pipeline failed: {args.Error}");

await taskManager.LinkActionToTasksAsync(commerceManager);

commerceManager.ProcessOrder(orderData);

```



