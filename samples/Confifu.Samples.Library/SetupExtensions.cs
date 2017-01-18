using System;
using Confifu.Abstractions;

namespace Confifu.Samples.Library
{
    public static class SetupExtensions
    {
        public static void SetupHelloWorldSayerFromEnvVars(this IAppConfig appConfig)
        {
            appConfig[Constants.OutputServiceKey] = ResolveConsoleOutput(appConfig.GetConfigVariables());
        }

        public static void SetupNullHelloWorldSayer(this IAppConfig appConfig)
        {
            appConfig[Constants.OutputServiceKey] = null;
        }

        private static IOutput ResolveConsoleOutput(IConfigVariables env)
        {
            var type = env["Confifu:Samples:Library:OutputType"];
            if (type == null)
                return null;

            switch (type.ToLower())
            {
                case "console":
                    return new ConsoleOutput();
                case "file":
                {
                    var file = env["Confifu:Samples:Library:OutputFile"];
                    return new FileOutput(file);
                }
                default:
                    return null;
            }
        }
    }

    internal class FileOutput : IOutput
    {
        public FileOutput(string file)
        {
            
        }

        public void Say(string msg)
        {
            
        }
    }

    public class ConsoleOutput : IOutput
    {
        public void Say(string msg)
        {
            //Console.WriteLine(msg);
        }
    }
}