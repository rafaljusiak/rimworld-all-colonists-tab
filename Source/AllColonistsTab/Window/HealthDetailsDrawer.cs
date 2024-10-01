using UnityEngine;
using Verse;

namespace AllColonistsTab.Window
{
    public static class HealthDetailsDrawer
    {
        public static void DrawHealthDetails(ref float curY, float width, Pawn_HealthTracker health)
        {
            if (health == null || health.hediffSet == null || health.hediffSet.hediffs == null)
            {
                Widgets.Label(new Rect(0, curY, width, Text.LineHeight), "Health information not available.");
                curY += Text.LineHeight + 2f;
                return;
            }

            var hediffs = health.hediffSet.hediffs;
            if (hediffs.Count > 0)
            {
                foreach (var hediff in hediffs)
                {
                    if (hediff == null)
                    {
                        continue;
                    }

                    string hediffLabel = hediff.LabelCap;
                    string bodyPartLabel = hediff.Part != null ? hediff.Part.LabelCap : "Whole body";
                    string severityLabel = hediff.SeverityLabel;

                    string label = $"{hediffLabel} ({bodyPartLabel})";
                    if (!string.IsNullOrEmpty(severityLabel))
                    {
                        label += $" - Severity: {severityLabel}";
                    }

                    Widgets.Label(new Rect(0, curY, width, Text.LineHeight), label);
                    curY += Text.LineHeight + 2f;
                }
            }
            else
            {
                Widgets.Label(new Rect(0, curY, width, Text.LineHeight), "No health conditions.");
                curY += Text.LineHeight + 2f;
            }
        }
    }
}
