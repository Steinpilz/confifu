using Confifu.Samples.Library;

namespace Confifu.Samples.App
{
    public class SampleAppSetup : AppSetup
    {
        public SampleAppSetup() : base(new CustomConfigVariables())
        {
            Common(() =>
            {
                AppConfig.SetupHelloWorldSayerFromEnvVars();
            });

            Environment("production", () =>
            {
                AppConfig.SetupNullHelloWorldSayer();
            });
        }
    }
}