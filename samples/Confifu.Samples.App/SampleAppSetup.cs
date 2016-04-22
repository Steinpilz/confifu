using Confifu.Samples.Library;

namespace Confifu.Samples.App
{
    public class SampleAppSetup : AppSetup
    {
        public static bool Setup;
        public SampleAppSetup() : base(new CustomConfigVariables())
        {
            Setup = true;
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