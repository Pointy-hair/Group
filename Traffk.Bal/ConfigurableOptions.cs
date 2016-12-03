using Microsoft.Extensions.Options;

namespace Traffk.Bal
{
    internal class ConfigurableOptions<TOptions> : IOptions<TOptions> where TOptions : class, new()
    {
        private readonly IOptions<TOptions> Options;
        private readonly IConfigureOptions<TOptions> Configurer;

        public ConfigurableOptions(IOptions<TOptions> options, IConfigureOptions<TOptions> configurer)
        {
            Options = options;
            Configurer = configurer;
        }

        TOptions IOptions<TOptions>.Value
        {
            get
            {
                var o = Options.Value;
                Configurer.Configure(o);
                return o;
            }
        }
    }
}
