namespace Confifu
{

    public static class AppEnvs
    {
        public static AppEnv Development = AppEnv.In("development");
        public static AppEnv Production = AppEnv.In("production");
        public static AppEnv Qa = AppEnv.In("qa");
        public static AppEnv Stage = AppEnv.In("stage");
        public static AppEnv Test = AppEnv.In("test");
    }
}