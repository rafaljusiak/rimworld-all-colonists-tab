using RimWorld;
using Verse;

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

        public static string GetRecordsFor(Pawn pawn)
        {
            if (pawn.records == null)
                return "No records found for this pawn.";

            var builder = new StringBuilder();

            foreach (var recordDef in DefDatabase<RecordDef>.AllDefs)
            {
                switch (recordDef.type)
                {
                    case RecordType.Int:
                        builder.AppendLine($"{recordDef.label}: {pawn.records.GetAsInt(recordDef)}");
                        break;
                    case RecordType.Time:
                        builder.AppendLine($"{recordDef.label}: {pawn.records.GetAsInt(recordDef)}");
                        break;
                    case RecordType.Float:
                        builder.AppendLine($"{recordDef.label}: {pawn.records.GetAsInt(recordDef)}");
                        break;
                }
            }

            return builder.ToString();
        }
    }
}

