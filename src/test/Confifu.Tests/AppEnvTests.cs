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

        [Fact]
        public void it_checks_complex_scenario_1()
        {
            var qa = AppEnv.In("qa");

            qa.IsIn("qa").ShouldBeTrue();

            var local = AppEnv.In("development") + AppEnv.In("test") + AppEnv.In("other-dev");

            local.IsIn("development").ShouldBeTrue();
            local.IsIn("test").ShouldBeTrue();
            local.IsIn("other-dev").ShouldBeTrue();
            local.IsIn("production").ShouldBeFalse();

            var deployed = !local;

            deployed.IsIn("development").ShouldBeFalse();
            deployed.IsIn("test").ShouldBeFalse();
            deployed.IsIn("other-dev").ShouldBeFalse();
            deployed.IsIn("production").ShouldBeTrue();

            var insideOtherNetwork = AppEnv.In("other-dev") + AppEnv.In("production") + AppEnv.In("other-qa");

            var deployedInsideOtherNetwork = insideOtherNetwork * deployed;
            deployedInsideOtherNetwork.IsIn("production").ShouldBeTrue();
            deployedInsideOtherNetwork.IsIn("other-qa").ShouldBeTrue();
            deployedInsideOtherNetwork.IsIn("other-dev").ShouldBeFalse();

            var notDeployedInOtherNetwork = !deployedInsideOtherNetwork;

            notDeployedInOtherNetwork.IsIn("qa").ShouldBeTrue();

        }

        [Fact]
        public void it_creates_all_app_env()
        {
            var sut = AppEnv.All;

            sut.IsIn("a").ShouldBeTrue();
            sut.IsIn("b").ShouldBeTrue();
            sut.IsIn("c").ShouldBeTrue();
            sut.IsIn("d").ShouldBeTrue();
        }

    }
}
