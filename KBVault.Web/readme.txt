# SignalR.Extras.Autofac

Directly addresses a current limitation of Autofac where it does not provide a mechanism to create a
lifetime-scope per SignalR hub invocation. SignalR already creates a separate hub instance for each call,
and Autofac is able to inject any dependencies, but neither library provides a way for those dependencies
lives to be bound to the lifetime of the hub. SignalR.Extras.Autofac provides a simple, transparent way to
bridge that gap.


## Usage:

1. Install the NuGet package: https://www.nuget.org/packages/SignalR.Extras.Autofac

2. Reference the namespace: SignalR.Extras.Autofac

3. When setting up an Autofac container in your project, follow the usual Autofac & SignalR integration
   steps as outlined on the Autofac wiki (http://autofac.readthedocs.org/en/latest/integration/signalr.html),
   i.e. replace SignalR's dependency resolver with Autofac's custom one and register your hubs as you
   normally would. If you're registering your hubs manually, you still need to configure them with
   ExternallyOwned().

4. Call the new RegisterLifetimeHubManager extension method on your ContainerBuilder instance, e.g.:

  builder.RegisterLifetimeHubManager();

5. Ensure that your SignalR hubs which require per-invocation lifetime scopes inherit from the
   LifetimeHub or LifetimeHub<T> classes.

Your hub instances will automatically and transparently be assigned their own new child lifetime scopes
upon each invocation by SignalR. They will also automatically dispose of those lifetime scopes upon
completion.

You can still register and use Hubs which do not inherit from LifetimeHub or LifetimeHub<T> - dependencies will
still be injected correctly by Autofac, however you will have to manually manage their lifetime scopes yourself
(as described here http://autofac.readthedocs.org/en/latest/integration/signalr.html#managing-dependency-lifetimes).

Note: disposing the Autofac container will result in any tracked LifetimeHub instances and their dependencies
also being disposed at that time.


## Example:

### Registration

// Create the container builder.
var builder = new ContainerBuilder();

// Register the LifetimeHub manager.
builder.RegisterLifetimeHubManager();

// Register the SignalR hubs.
builder.RegisterHubs(Assembly.GetExecutingAssembly());

// Register other dependencies.
builder.Register(c => new UnitOfWork()).As<IUnitOfWork>().InstancePerLifetimeScope();

// Build the container.
var container = builder.Build();

// Configure SignalR with an instance of the Autofac dependency resolver.
GlobalHost.DependencyResolver = new AutofacDependencyResolver(container);

### Your hubs

public class MyHub : LifetimeHub
{
    public MyHub(IUnitOfWork unitOfWork) {
        _unitOfWork = unitOfWork;
    }

    public void DoSomething() {
        //The hub instance and dependencies like UnitOfWork are automatically created prior to SignalR invoking this method
        //Do stuff here
        //The hub instance and dependencies like UnitOfWork are automatically destroyed after SignalR has invoked this method
    }

    private IUnitOfWork _unitOfWork;
}

public class SomeHub : LifetimeHub<ISomeClient>
{
    public SomeHub(IUnitOfWork unitOfWork) {
        _unitOfWork = unitOfWork;
    }

    public void DoSomething() {
        //The hub instance and dependencies like UnitOfWork are automatically created prior to SignalR invoking this method
        //Do stuff here
        //The hub instance and dependencies like UnitOfWork are automatically destroyed after SignalR has invoked this method
    }

    private IUnitOfWork _unitOfWork;
}
