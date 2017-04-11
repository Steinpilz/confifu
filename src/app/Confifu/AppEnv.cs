using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Confifu
{
    /// <summary>
    /// Represents AppEnv logic
    /// </summary>
    public class AppEnv
    {
        class AppEnvExpression
        {
            AppEnv Left { get; }
            AppEnv Right { get; }
            AppEnvOperation Op { get; }

            internal AppEnvExpression(AppEnvOperation op, AppEnv left, AppEnv right)
            {
                Op = op;
                Left = left;
                Right = right;
            }

            internal bool IsIn(string testEnv) => Op.Match(
                    union: () => Left.IsIn(testEnv) || Right.IsIn(testEnv),
                    intersect: () => Left.IsIn(testEnv) && Right.IsIn(testEnv),
                    negative: () => !Left.IsIn(testEnv)
                    );

            public override string ToString()
            {
                return Op.Match(
                    union: () => $"{Left} + {Right}",
                    intersect: () => $"{Left.ToExpressionString()} * {Right.ToExpressionString()}",
                    negative: () => $"!{Left.ToExpressionString()}"
                    );
            }
        }

        class AppEnvOperation 
        {
            public static AppEnvOperation Union = new AppEnvOperation(1);
            public static AppEnvOperation Intersect = new AppEnvOperation(2);
            public static AppEnvOperation Negative = new AppEnvOperation(3);

            int value;
            private AppEnvOperation(int value)
            {
                this.value = value;
            }

            public T Match<T>(Func<T> union, Func<T> intersect, Func<T> negative)
            {
                if (this.Equals(Union))
                    return union();
                if (this.Equals(Intersect))
                    return intersect();
                if (this.Equals(Negative))
                    return negative();

                Debug.Assert(false, "Unknown AppEnvOperation");
                throw new Exception("wtf!!"); // support compiler, actually must not happen
            }

            public override bool Equals(object obj)
            {
                var op = obj as AppEnvOperation;
                if (op == null) return false;
                return op.value == this.value;
            }

            public override int GetHashCode() => value.GetHashCode();
        }

        private HashSet<string> include;
        private AppEnvExpression expression;

        private AppEnv(IEnumerable<string> include)
        {
            this.include = new HashSet<string>(include, StringComparer.CurrentCultureIgnoreCase);
        }

        private AppEnv(AppEnvExpression expression)
        {
            this.expression = expression;
        }

        internal string ToExpressionString() => IsExpression() ? $"({this})" : this.ToString();

        internal bool IsExpression() => expression != null;

        /// <summary>
        /// AppEnv instance containing All environments
        /// </summary>
        /// <returns></returns>
        public static AppEnv All = !new AppEnv(new string[0]);

        /// <summary>
        /// Creates AppEnv instance with single <paramref name="appEnvs"/> environment
        /// </summary>
        /// <param name="appEnvs"></param>
        /// <returns></returns>
        public static AppEnv In(params string[] appEnvs) => new AppEnv(appEnvs);

        /// <summary>
        /// Creates AppEnv instance contains all environments except <paramref name="appEnvs"/>
        /// </summary>
        /// <param name="appEnvs"></param>
        /// <returns></returns>
        public static AppEnv NotIn(params string[] appEnvs) => !In(appEnvs);

        /// <summary>
        /// Creates new AppEnv containing both environments
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        public AppEnv Plus(AppEnv another)
            => new AppEnv(new AppEnvExpression(AppEnvOperation.Union, this, another));

        /// <summary>
        /// Creates new AppEnv containing intersection of both environments
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        public AppEnv Intersects(AppEnv another)
            => new AppEnv(new AppEnvExpression(AppEnvOperation.Intersect, this, another));

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
        public AppEnv Negative() => new AppEnv(new AppEnvExpression(AppEnvOperation.Negative, this, null));

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
            if (expression != null)
                return expression.IsIn(environment);

            return include.Contains(environment);
        }

        public override string ToString()
        {
            if (expression != null)
                return expression.ToString();

            return "[" + string.Join(",", include)+ "]";
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