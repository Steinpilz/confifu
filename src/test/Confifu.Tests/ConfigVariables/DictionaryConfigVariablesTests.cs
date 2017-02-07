using Shouldly;
using System.Collections.Generic;
using Confifu.ConfigVariables;
using Xunit;

namespace Confifu.Tests.ConfigVariables
{

    public class DictionaryConfigVariablesTests
    {
        [Fact]
        public void it_returns_values_from_given_dictionary()
        {
            var sut = Sut(new Dictionary<string, string> {
                ["prop"] = "value",
            });

            sut["prop"].ShouldBe("value");
        }

        [Fact]
        public void it_returns_null_if_key_is_not_present_in_given_dictionary()
        {
            var sut = Sut(new Dictionary<string, string>
            {
                ["prop"] = "value",
            });

            sut["prop1"].ShouldBeNull();
        }

        private DictionaryConfigVariables Sut(IDictionary<string ,string> dictionary)
        {
            return new DictionaryConfigVariables(dictionary);
        }
    }

    public class DictionaryConfigVariablesBuilderTests : ConfigVariablesBuilderTestsBase
    {
        [Fact]
        public void it_creates_valid_config_variables()
        {
            var vars = Build(builder =>
            {
                builder.Dictionary(d =>
                {
                    d.Add("1", "2");
                    d.Add("2", "3");
                });
            });

            vars["1"].ShouldBe("2");
            vars["2"].ShouldBe("3");
            vars["3"].ShouldBeNull();
        }

        [Fact]
        public void it_creates_valid_config_variables_using_cool_configurator()
        {
            var vars = Build(builder =>
            {
                builder.Static(d =>
                {
                    d("1", "2");
                    d("2", "3");
                });
            });

            vars["1"].ShouldBe("2");
            vars["2"].ShouldBe("3");
            vars["3"].ShouldBeNull();
        }
    }
}