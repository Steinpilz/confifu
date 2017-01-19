using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Confifu.LibraryConfig
{
    internal static class PropertyName
    {
        public static MemberExpression GetMemberExpression(this Expression expression)
        {
            MemberExpression memberExpression = null;
            if (expression is LambdaExpression)
            {
                var lambda = (LambdaExpression)expression;
                memberExpression = lambda.Body.GetMemberExpression();
            }
            else if (expression is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)expression;
                memberExpression = unaryExpression.Operand.GetMemberExpression();
            }
            else if (expression is MemberExpression)
                memberExpression = (MemberExpression)expression;

            return memberExpression;
        }

        public static MemberInfo GetMemberInfo(this Expression expression)
        {
            MemberExpression memberExpression;

            if (expression is LambdaExpression)
            {
                var lambda = (LambdaExpression)expression;
                memberExpression = GetMemberExpression(lambda.Body);
            }
            else
            {
                memberExpression = GetMemberExpression(expression);
            }

            return memberExpression.Member;
        }

        public static string GetPropertyNameCascade(this Expression expression)
        {
            var propertiesList = new List<string>();
            for (var iExpression = expression;
                iExpression != null
                && iExpression.GetMemberExpression() != null;
                iExpression = iExpression.GetMemberExpression().Expression)
            {
                propertiesList.Add(iExpression.GetPropertyName());
            }

            propertiesList.Reverse();
            return String.Join(".", propertiesList);
        }

        public static string GetPropertyName(this Expression expression)
        {
            return expression.GetMemberInfo().Name;
        } 

        public static string Resolve<T>(Expression<Func<T, object>> property)
        {
            return property.GetPropertyNameCascade();
        }

        public static string Resolve<TSource, TDest>(Expression<Func<TSource, TDest>> property )
        {
            return property.GetPropertyNameCascade();
        }
    }
}