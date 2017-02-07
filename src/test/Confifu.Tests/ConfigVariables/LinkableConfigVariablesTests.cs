using Confifu.ConfigVariables;
using Shouldly;
using System;
using Xunit;

namespace Confifu.Tests.ConfigVariables
{
    public class LinkableConfigVariablesTests : ConfigVariablesBuilderTestsBase
    {
        [Fact]
        public void it_supports_links_inside_config_values()
        {
            var vars = Build(builder => builder.Linkable(
                childBuilder =>
            {
                childBuilder.Static(x =>
                {
                    x("prop1", "val1");
                    x("prop2", "{prop1}");
                });
            }));

            vars["prop2"].ShouldBe("val1");
        }

        [Fact]
        public void it_supports_multi_level_links_inside_config_values()
        {
            var vars = Build(builder => builder.Linkable(
                childBuilder =>
                {
                    childBuilder.Static(x =>
                    {
                        x("prop1", "val1");
                        x("prop2", "{prop1}");
                        x("prop3", "{prop2}-val2");
                    });
                }));

            vars["prop3"].ShouldBe("val1-val2");
        }
        
        [Fact]
        public void it_detects_circular_dependency()
        {
            var vars = Build(builder => builder.Linkable(
                           childBuilder =>
                           {
                               childBuilder.Static(x =>
                               {
                                   x("prop1", "{prop3}");
                                   x("prop2", "{prop1}");
                                   x("prop3", "{prop2}-val2");
                               });
                           }));

            Assert.Throws<InvalidOperationException>(() => vars["prop2"]);
        }

        [Fact]
        public void it_looks_linked_keyvs_using_nesting_rules()
        {
            // commonly useful in json config:
            /// {
            ///     "SomeModule" : {
            ///         "Prop1": "abc",
            ///         "Prop2": "some text here from Prop1 - {Prop1}"
            ///     }
            /// }
            ///

            var vars = Build(builder => builder.Linkable(
                childBuilder =>
                {
                    childBuilder.Static(x =>
                    {
                        x("prop1", "val1");
                        x("prop3", "val3");
                        x("prop4", "{prop1}");
                        x("child:prop1", "child-val");
                        x("child:prop2", "{prop1}");
                        x("child:child:prop1", "child-child-val");
                        x("child:child:prop2", "{prop3}");
                        x("child:child:prop5", "{prop4}");
                    });
                }));

            vars["child:prop2"].ShouldBe("child-val");
            vars["child:child:prop2"].ShouldBe("val3");
            vars["child:child:prop5"].ShouldBe("val1");
        }
    }
}
