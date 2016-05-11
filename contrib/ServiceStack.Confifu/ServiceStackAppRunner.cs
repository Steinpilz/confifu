using System;

namespace ServiceStack.Confifu
{
    public class ServiceStackAppRunner
    {
        private readonly ServiceStackConfig _config;

        public ServiceStackAppRunner(ServiceStackConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            _config = config;
        }

        public void Run()
        {
            ValidateConfig();

            if (_config.AppHost == null)
                return;

            var appHost = _config.AppHost();

            appHost.Run(endpointConfig =>
            {
                _config.Config?.Invoke(appHost, endpointConfig);
            });
        }

        private void ValidateConfig()
        {

        }
    }
}