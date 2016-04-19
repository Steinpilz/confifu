using Confifu.Abstractions;

namespace Confifu.Samples.Library
{
    public class HelloWorldSayer
    {
        public void Say()
        {
            var output = App.Config.Get<IOutput>(Constants.OutputServiceKey);
            output.Say("Hello World");
        }
    }
}