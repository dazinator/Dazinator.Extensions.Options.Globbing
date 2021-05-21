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


# [Getting Started]
- Clone this repo, then push to your own origin.
- Create your solution (.sln) and projects in the `/src` directory.
- Make sure global.json has the right version of the .net sdk that you require.
- If you want to run the `dotnet-format` and `gitversion` tools (that are used as part of the CI builds) locally, then install them by running the following command in the repo root directory:
    `dotnet tool restore`
- For AppVeyor builds, update AppVeyor.yml:
    - dotnet sdk version (currently set to install latest pre-release).
    - Now you can add to AppVeyor.
- For Azure Devops builds:
    - Import pipelines yaml file into Azure Devops pipeline.
- For GitHub Actions - the workflow file is detected automatically when you push up and should be run.
