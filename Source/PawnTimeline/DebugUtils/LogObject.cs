using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PawnTimeline.DebugUtils
{
    public static class LogObject
    {
        public static string GetLogFor(object target)
        {
            var properties =
                from property in target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                select new
                {
                    Name = property.Name,
                    Value = GetValueSafely(property, target)
                };

            var builder = new StringBuilder();

            foreach (var property in properties)
            {
                builder
                    .Append(property.Name)
                    .Append(" = ")
                    .Append(property.Value)
                    .AppendLine();
            }

            return builder.ToString();
        }

        private static string GetValueSafely(PropertyInfo property, object target)
        {
            try
            {
                var value = property.GetValue(target, null);
                return value?.ToString() ?? "null";
            }
            catch (Exception ex)
            {
                return $"Error retrieving value: {ex.Message}";
            }
        }
    }
}

