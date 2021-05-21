namespace Dazinator.Extensions.Options.Globbing
{
    using System.Collections.Generic;
    using System.Linq;
    using DotNet.Globbing;
    using Microsoft.Extensions.Options;

    public class GlobMatchingNamedOptionsFactory<TOptions> : IOptionsFactory<TOptions> where TOptions : class
    {
        private readonly IOptionsFactory<TOptions> _innerFactory;
        private Dictionary<string, Glob> _globs = null;

        public GlobMatchingNamedOptionsFactory(
            IEnumerable<IConfigureOptions<TOptions>> setups,
            IOptionsFactory<TOptions> innerFactory)
        {
            _innerFactory = innerFactory;
            // _optionsMonitor = optionsMonitor;

            // discover registered names.
            var names = new HashSet<string>();

            foreach (var item in setups)
            {
                if (item is ConfigureNamedOptions<TOptions> namedSetup)
                {
                    names.Add(namedSetup.Name);
                }
            }

            LoadGlobs(names);
        }

        private void LoadGlobs(IEnumerable<string> names)
        {
            var globs = new Dictionary<string, Glob>();
            foreach (var name in names)
            {
                // detect names that are glob patterns.
                if (!string.IsNullOrWhiteSpace(name))
                {
                    var glob = Glob.Parse(name);
                    if (glob.Tokens.Any(token => (!(token is DotNet.Globbing.Token.LiteralToken ||
                                                    token is DotNet.Globbing.Token.PathSeparatorToken))))
                    {
                        globs.Add(name, glob);
                    }
                }

            }

            _globs = globs;
        }

        public TOptions GetMatchingOptions(string name)
        {
            foreach (var key in _globs.Keys)
            {
                var glob = _globs[key];
                if (glob.IsMatch(name))
                {
                    var targetOptions = _innerFactory.Create(key);
                    //  _targetOptionsMonitor.Get(key);
                    return targetOptions;
                }
            }

            return _innerFactory.Create(name);
        }

        public TOptions Create(string name)
        {
            return GetMatchingOptions(name);
        }
    }
}
