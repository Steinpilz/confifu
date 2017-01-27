using System;
using Confifu.Samples.App;
using Confifu;

namespace Confifu.Samples.App
{

    public class Startup
    {
        public static void Main(params string[] args)
        {
            Console.WriteLine("Setup = " + SampleAppSetup.Setup);
            Console.Read();
        } 
    }
}