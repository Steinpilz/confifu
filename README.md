# Confifu

Confifu was created by inspiration of application configuration in Ruby On Rails. It provides a simple way to configure your .Net application or module for different environments. It's convention based and also provides simple abstractions to create libraries and loosely coupled modules on top of it.

### Install

Confifu is distributed as 2 nuget packages:

- https://www.nuget.org/packages/Confifu.Abstractions
- https://www.nuget.org/packages/Confifu

So they can be installed by:

```
Install-Package Confifu
Install-Package Confifu.Abstractions
```

### Abstractions

```Confifu.Abstractions``` contains main abstractions which are supposed to be a core of an application config

1. **IAppConfig**

```csharp
public interface IAppConfig
{
    object this[string key] { get; set; }
}
```

The instance of ```IAppConfig``` is considered to be the only single place where configuration are stored to. So anytime at setup phase any module can set and read config properties.

2. **IConfigVariables**

```csharp
public interface IConfigVariables
{
    string this[string key] { get; }
}
```

Very simple abstraction to provide access to key value configuration variables. By convention ```IAppConfig``` is supposed to have a single instance of ```IConfigVariables``` so all modules could use it and application at the top level can set any ```IConfigVariables``` it needs. It could be environment variables, app.config's appSettings section, json file, combination of them, any class that impleents ```IConfigVariables```.

It's stored in the ```IAppConfig``` under **ConfigVariables** key. There are also extensions methods to help working with it:

```csharp
appConfig.GetConfigVariables();
appConfig.SetConfigVariables(configVariables);
```

3. **AppRunner**

AppRunner is supposed to be an ```Action``` which "Run" the IAppConfig instance. By convention it's the only one place where configuration could expose to outer world (use static variables). It becomes usefull for integration of existing libraries to Confifu. For example [integration](https://github.com/Steinpilz/fluentscheduler-confifu/blob/master/src/app/FluentScheduler.Confifu/AppConfigExtensions.cs) of [FluentScheduler](https://github.com/fluentscheduler/FluentScheduler) to IAppConfig:

```csharp

appConfig.AddAppRunnerAfter(() =>
{
    var actualRegistry = appConfig.GetFluentSchedulerRegistry();
    var serviceProvider = appConfig.GetServiceProvider();
    if (serviceProvider != null)
        JobManager.JobFactory = new ServiceProviderJobFactory(serviceProvider);

    JobManager.Initialize(actualRegistry);
});
                
```

Here we have configured schedulerRegistry through confifu, but to make it run we should call ```JobManager.Initialize```. 
There are also some usefull extensions methods to work with AppRunner:

```csharp
appConfig.GetAppRunner();
appConfig.SetAppRunner(runner);
appConfig.WrapAppRunner(currentRunner => () => {
// make some stuff before running already configured runner
currentRunner();
// make some stuff after running already configured runner
});
appConfig.AddAppRunnerAfter(runner);
appConfig.AddAppRunnerBefore(runner);
```

### Samples



### Usage in Application

Add new class inherited from `Confifu.AppSetup` which is going to setup your application:
```csharp

```
