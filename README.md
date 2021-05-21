[![.NET](https://github.com/dazinator/Dazinator.Extensions.Options.Globbing/actions/workflows/dotnet.yml/badge.svg)](https://github.com/dazinator/Dazinator.Extensions.Options.Globbing/actions/workflows/dotnet.yml)


## Features

When configuring named options, you usually have to configure each name individually.

```csharp
 services.Configure<TestOptions>("plugin-a", (a) =>
            {
                a.SomeSetting = false;
            });
            
 services.Configure<TestOptions>("plugin-b", (a) =>
            {
                a.SomeSetting = false;
            });
  services.Configure<TestOptions>("plugin-c", (a) =>
            {
                a.SomeSetting = true;
            });
```

Then later in your application you can obtain the settings for each plugin.

```csharp
var options = _optionsMonitor.Get("plugin-a");
```

This is ok. But what if you want to ensure you have one set of configuration to be applied to multiple names?
This library helps you do that by allowing you to register your option names using a glob pattern, which will be applied to any matching options name.

Example:

```csharp
    services.Configure<TestOptions>("plugin-[ab]", (a) =>
            {
                a.SomeSetting = true;
            });            
    services.Configure<TestOptions>("plugin-*", (a) =>
            {
                a.SomeSetting = false;
            });
    services.AddGlobMatchingNamedOptions<TestOptions>(); // important!  
```

Notice the use of the `DotNet.Glob` pattern when configuring the named option.
Now when you request an options with a specific name, the first named configuration that matches that name will be used to configure that named instance.

```csharp

    var pluginOptions = sut.Get("plugin-a");
    Assert.True(pluginOptions.SomeSetting);
    
    var otherOptions = sut.Get("plugin-d");
    Assert.False(otherOptions.SomeSetting);   

```
