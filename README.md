[![standard-readme compliant][]][standard-readme]
[![NuGet package][nugetimage]][nuget]
[![Maintainability][Maintainability_Badge]][Maintainability]

# MediatR.LightCore
Implementation of the registration module of LightCore for registration of MediatR. 

## Install

```
PM> Install-Package MediatR.LightCore
```

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

## Contributing

PRs accepted.

## License
[MIT License © Jürgen Rosenthal-Buroh](license.txt)

[Maintainability_Badge]: https://api.codeclimate.com/v1/badges/e419658128e259a51447/maintainability
[Maintainability]: https://codeclimate.com/github/juro-org/MediatR.LightCore/maintainability
[standard-readme]: https://github.com/RichardLitt/standard-readme
[standard-readme compliant]: https://img.shields.io/badge/readme%20style-standard-brightgreen.svg?style=flat-square
[nuget]: https://nuget.org/packages/MediatR.LightCore
[nugetimage]: https://img.shields.io/nuget/v/MediatR.LightCore.svg?logo=nuget&style=flat-square
