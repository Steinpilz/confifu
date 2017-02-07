using System.Collections.Generic;
using Xunit;
using Confifu;
using Confifu.Abstractions;
using FluentScheduler.Confifu;
using FluentScheduler;
using Confifu.ConfigVariables;
using System.Diagnostics;
using System.Threading;

namespace FluentScheduler.Confifu.Tests
{
    public class ConfigTests
    {
        [Fact]
        public void it_does_not_smoke()
        {
            var appConfig = CreateAppConfig();
            appConfig.UseFluentScheduler(r =>
            {

            });
        }

        [Fact]
        public void it_schedules_and_run_task()
        {
            var itIsWorking = false;
            var appConfig = CreateAppConfig();
            appConfig.UseFluentScheduler(r =>
            {
                r.Schedule(() => {
                    itIsWorking = true;
                }).ToRunNow();
            });
            appConfig.GetAppRunner()?.Invoke();
        
            Thread.Sleep(100);
            Assert.True(itIsWorking);
        }
        
        private static AppConfig CreateAppConfig()
        {
            var appConfig = new AppConfig();
            appConfig.SetConfigVariables(new DictionaryConfigVariables(new Dictionary<string, string>()));
            return appConfig;
        }

    }
}