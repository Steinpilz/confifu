using System;
using System.Collections.Generic;
using System.Linq;

namespace Confifu
{
    /// <summary>
    /// Represents AppEnv logic
    /// </summary>
    public class AppEnv
    {
        private HashSet<string> include;
        private HashSet<string> exclude;

        private AppEnv(IEnumerable<string> include, IEnumerable<string> exclude)
        {
            this.include = include == null ? null : new HashSet<string>(include, StringComparer.CurrentCultureIgnoreCase);
            this.exclude = exclude == null ? null : new HashSet<string>(exclude, StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// AppEnv instance containing All environments
        /// </summary>
        /// <returns></returns>
        public static AppEnv All = new AppEnv(null, null);

        /// <summary>
        /// Creates AppEnv instance with single <paramref name="appEnvs"/> environment
        /// </summary>
        /// <param name="appEnvs"></param>
        /// <returns></returns>
        public static AppEnv In(params string[] appEnvs) => new AppEnv(appEnvs, null);

        /// <summary>
        /// Creates AppEnv instance contains all environments except <paramref name="appEnvs"/>
        /// </summary>
        /// <param name="appEnvs"></param>
        /// <returns></returns>
        public static AppEnv NotIn(params string[] appEnvs) => new AppEnv(null, appEnvs);

        /// <summary>
        /// Creates new AppEnv containing both environments
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        public AppEnv Plus(AppEnv another)
            => new AppEnv(MergeInclude(include, another.include, (a,b) => a.Concat(b)), 
                    MergeExclude(exclude, another.exclude, (a,b) => a.Intersect(b)));

        /// <summary>
        /// Creates new AppEnv containing intersection of both environments
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        public AppEnv Intersects(AppEnv another)
            => new AppEnv(MergeInclude(include, another.include, (a, b) => a.Intersect(b)),
                MergeExclude(exclude, another.exclude, (a, b) => a.Concat(b)));

        /// <summary>
        /// Creates new AppEnv containing instance environments except of environments from <paramref name="another"/>
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        public AppEnv Minus(AppEnv another) => this.Intersects(another.Negative());

        /// <summary>
        /// Creates new AppEnv containing Reverse environments
        /// </summary>
        /// <returns></returns>
        public AppEnv Negative() => new AppEnv(exclude, include);

        public static AppEnv operator +(AppEnv appEnv1, AppEnv appEnv2) => appEnv1.Plus(appEnv2);
        public static AppEnv operator *(AppEnv appEnv1, AppEnv appEnv2) => appEnv1.Intersects(appEnv2);
        public static AppEnv operator -(AppEnv appEnv1, AppEnv appEnv2) => appEnv1.Minus(appEnv2);
        public static AppEnv operator !(AppEnv appEnv1) => appEnv1.Negative();

        public static implicit operator AppEnv (string s)
        {
            return AppEnv.In(s);
        }

        /// <summary>
        /// Check is <paramref name="environment"/> in
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        public bool IsIn(string environment)
        {
            if (exclude != null && exclude.Contains(environment))
                return false;

            if (include != null && !include.Contains(environment))
                return false;

            return true;
        }

        public override string ToString()
        {
            return $"[{string.Join(",", include)}],-[{string.Join(",", exclude)}]";
        }

        private static IEnumerable<string> MergeInclude(IEnumerable<string> first, IEnumerable<string> second,
            Func<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> mergeFunc)
        {
            // null means all set

            if (first == null || second == null)
                return null;

            return mergeFunc(first, second);
        }

        private static IEnumerable<string> MergeExclude(IEnumerable<string> first, IEnumerable<string> second,
            Func<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> mergeFunc)
        { 
            if (first == null && second == null)
                return null;

            if (first == null)
                return second;

            if (second == null)
                return first;

            return mergeFunc(first, second);
        }
    }
}