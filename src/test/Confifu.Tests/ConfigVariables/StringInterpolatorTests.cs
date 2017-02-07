using Confifu.ConfigVariables;
using Shouldly;
using Xunit;

namespace Confifu.Tests.ConfigVariables
{

    public class StringInterpolatorTests
    {
        [Theory]
        [InlineData("{var1}", "var1")]
        [InlineData("{{var1}", "{var1")]
        [InlineData("{{{var1}", "{var1")]
        [InlineData("{{{var1}}}", "{var1}")]
        [InlineData("{{{var1}}} {{{var2}}}", "{var1} {var2}")]
        public void it_interpolates_simple_variable(string input, string expected)
        {
            var sut = new StringInterpolator(input, x => x);

            sut.Interpolate().ShouldBe(expected);
        }
    }
}