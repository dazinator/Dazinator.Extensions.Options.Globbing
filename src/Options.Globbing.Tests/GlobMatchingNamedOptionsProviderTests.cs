namespace Dazinator.Extensions.Options.Globbing.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Xunit;

    public class GlobMatchingNamedOptionsProviderTests
    {
        [Fact]
        public void Can_Get_CompartmentOptions_When_PatternRegistered()
        {
            var services = new ServiceCollection();
            services.Configure<TestOptions>("plugin-[ab]", (a) =>
            {
                a.SomeSetting = true;
            });
            services.Configure<TestOptions>("plugin-*", (a) =>
            {
                a.SomeSetting = false;
            });
            services.AddGlobMatchingNamedOptions<TestOptions>();
            var sp = services.BuildServiceProvider();
            var sut = sp.GetRequiredService<IOptionsMonitor<TestOptions>>();

            var pluginOptions = sut.Get("plugin-a");
            Assert.True(pluginOptions.SomeSetting);

            var otherOptions = sut.Get("plugin-d");
            Assert.False(otherOptions.SomeSetting);
        }
    }

    public class TestOptions
    {
        public bool SomeSetting { get; set; }
    }
}
