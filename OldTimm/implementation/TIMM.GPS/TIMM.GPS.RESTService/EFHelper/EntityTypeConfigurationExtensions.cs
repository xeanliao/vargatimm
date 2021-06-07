using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration;

namespace TIMM.GPS.RESTService.EFHelper
{
    /// <summary>
    /// EF actually doesn’t care about visibility of the property setter. 
    /// It can be private if you want it to be. 
    /// Add string parameter for code only Fluent API to mapping property
    /// eg.
    /// Property("MyPrivateStringProperty").HasColumnName("FooString");
    /// http://blogs.msdn.com/b/schlepticons/archive/2011/08/31/map-private-properties-with-ef-fluent-api.aspx
    /// </summary>
    public static class EntityTypeConfigurationExtensions
    {
        private static Expression<Func<T, K>> CreateExpression<T, K>(String propertyName)
        {
            var type = typeof(T);
            var arg = Expression.Parameter(type, "x");
            Expression expr = arg;

            var pi = type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (pi == null) throw new ArgumentException(String.Format("Property '{0}' on type '{1}' cannot be found.", propertyName, type.FullName));

            expr = Expression.Property(expr, pi);

            LambdaExpression lambda = Expression.Lambda(expr, arg);

            var expression = (Expression<Func<T, K>>)lambda;

            return expression;
        }

        public static StringPropertyConfiguration Property<T>(this StructuralTypeConfiguration<T> mapper, String propertyName) where T : class
        {
            var expression = CreateExpression<T, String>(propertyName);

            return mapper.Property(expression);
        }

        public static PrimitivePropertyConfiguration Property<T, K>(this StructuralTypeConfiguration<T> mapper, String propertyName)

            where T : class

            where K : struct
        {
            var expression = CreateExpression<T, K>(propertyName);

            return mapper.Property(expression);
        }
    }
}
