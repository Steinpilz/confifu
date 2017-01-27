using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Confifu.Tests
{
    public class AppSetupTests
    {
         
    }

    public static class XUnitShim
    {
        static XUnitShim()
        {
            var x = typeof(TestFrameworkOptionsReadExtensions);
        }
    }
}