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

    public class ConfigVariablesChainTests 
    {
        
    }
}