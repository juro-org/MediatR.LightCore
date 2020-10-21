# MediatR.LightCore
Implementation of the registration module of LightCore for registration of MediatR. 

## Usage

See also the [sample project](https://github.com/JuergenRB/MediatR/blob/master/samples/MediatR.Examples.LightCore/Program.cs) for a running example.

### RegisterModule

```cs
var builder = new ContainerBuilder();
builder.RegisterModule(new MediatRModule(typeof(StartUp).Assembly));

// Register even more stuff

var container = builder.Build();
var mediator = container.Resolve<IMediator>();
```
## License
[MIT License © Jürgen Rosenthal-Buroh](license.txt)
