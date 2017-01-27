using Confifu.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Confifu.Samples.Library.A.Setup
{
    public class Builder
    {
        internal Builder(IAppConfig appConfig)
        {

        }

        public void EnsureDefault()
        {

        }
    }

    public static class Exts
    {
        public static IAppConfig UseLibraryA(this IAppConfig appConfig, Action<Builder> buildAction)
        {
            var builder = new Builder(appConfig);
            builder.EnsureDefault();

            buildAction(builder);

            return appConfig;
        }
    }
}
