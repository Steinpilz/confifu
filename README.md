# confifu
Confifu was created by inspiration of application configuration in Ruby On Rails. It gives a simple way to configure any .Net project

### Abstractions

### Dependency Injection

### LibraryConfig

### Samples

### Install

```
Install-Package Confifu
```

### Usage in Application

Add new class inherited from `Confifu.AppSetup` which is going to setup your application:
```csharp
public class App : AppSetup 
{
  public App(IConfigVariables vars): base(vars)
  {
    // here we describing common setup action which will be run on setup phase
    Common(() => {
      // do something with AppConfig
    });
    
    // this action will be called only for development environment
    Environment(() => {
    // do something with AppConfig
    }, CSharpEnv.Development);
    
    // this action will be calle only for test environment
    Environment(()=> {
    // do something with AppConfig
    }, CSharpEnv.Test);
    
    // this action will be called for staging and production environment
    Environment(() => {
    // do something with AppConfig
    }, CSharpEnv.Staging, CSharpEnv.Production);
  }
}
```
