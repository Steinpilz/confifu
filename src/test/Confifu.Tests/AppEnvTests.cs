using Shouldly;
using Xunit;

namespace Confifu.Tests
{
    public class AppEnvTests
    {
        private AppEnv E(string s)
        {
            return (AppEnv)s;
        }


        private AppEnv NotE(string s)
        {
            return AppEnv.NotIn(s);
        }


        [Fact]
        public void it_checks_is_in_correctly()
        {
            var env = AppEnv.In("A");

            env.IsIn("A").ShouldBeTrue();
            env.IsIn("B").ShouldBeFalse();
        }

        [Fact]
        public void it_checks_is_in_case_insensitive()
        {
            var env = AppEnv.In("A");

            env.IsIn("a").ShouldBeTrue();
            env.IsIn("b").ShouldBeFalse();
        }

        [Fact]
        public void not_in_works()
        {
            var env1 = AppEnv.NotIn("A");

            env1.IsIn("A").ShouldBeFalse();
            env1.IsIn("B").ShouldBeTrue();
            env1.IsIn("C").ShouldBeTrue();
        }

        [Fact]
        public void plus_operator_works()
        {
            var env1 = AppEnv.In("A");
            var env2 = AppEnv.In("B");

            (env1 + env2).IsIn("A").ShouldBeTrue();
            (env1 + env2).IsIn("B").ShouldBeTrue();
            (env1 + env2).IsIn("C").ShouldBeFalse();
        }

        [Fact]
        public void plus_operator_works_with_not_in()
        {
            (E("A") + NotE("B")).IsIn("A").ShouldBeTrue();
            (E("A") + NotE("B")).IsIn("B").ShouldBeFalse();
            (E("A") + NotE("B")).IsIn("C").ShouldBeTrue();
        }


        [Fact]
        public void plus_operator_works_with_not_in_2()
        {
            (NotE("A") + NotE("B")).IsIn("A").ShouldBeTrue();
            (NotE("A") + NotE("B")).IsIn("B").ShouldBeTrue();
            (NotE("A") + NotE("B")).IsIn("C").ShouldBeTrue();
            (NotE("A") + NotE("B")).IsIn("D").ShouldBeTrue();
        }

        [Fact]
        public void negative_operator_works()
        {
            (!E("A")).IsIn("A").ShouldBeFalse();
            (!E("A")).IsIn("B").ShouldBeTrue();
        }


        [Fact]
        public void negative_operator_works_2()
        {
            (!NotE("A")).IsIn("A").ShouldBeTrue();
            (!NotE("A")).IsIn("B").ShouldBeFalse();
        }

        [Fact]
        public void intersects_works()
        {
            var actual = AppEnv.In("A", "B", "C") * AppEnv.In("B", "D");

            actual.IsIn("A").ShouldBeFalse();
            actual.IsIn("B").ShouldBeTrue();
            actual.IsIn("C").ShouldBeFalse();
            actual.IsIn("D").ShouldBeFalse();
        }

        [Fact]
        public void minus_operator_works()
        {
            var actual = AppEnv.In("A", "B", "C") - AppEnv.In("B", "D");

            actual.IsIn("A").ShouldBeTrue();
            actual.IsIn("B").ShouldBeFalse();
            actual.IsIn("C").ShouldBeTrue();
            actual.IsIn("D").ShouldBeFalse();
        }
    }
}
