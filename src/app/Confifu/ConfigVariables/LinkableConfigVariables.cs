using Confifu.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Confifu.ConfigVariables
{
    public class LinkableConfigVariables : IConfigVariables
    {
        public string this[string key] => null;

        public LinkableConfigVariables(IConfigVariables underlying)
        {
            if (underlying == null)
                throw new ArgumentNullException(nameof(underlying));
        }
    }
}
