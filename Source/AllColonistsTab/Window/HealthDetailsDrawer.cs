using UnityEngine;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace AllColonistsTab.Window
{
    public static class HealthDetailsDrawer
    {
        public static void DrawHealthDetails(ref float curY, float width, Pawn pawn)
        {
            var health = pawn.health;

            if (health == null || health.hediffSet == null)
            {
                Widgets.Label(new Rect(0, curY, width, Text.LineHeight), "Health information not available.");
                curY += Text.LineHeight + 2f;
                return;
            }

            List<Hediff> hediffs = health.hediffSet.hediffs;

            if (hediffs == null || hediffs.Count == 0)
            {
                Widgets.Label(new Rect(0, curY, width, Text.LineHeight), "No significant health conditions.");
                curY += Text.LineHeight + 2f;
                return;
            }

            var importantHediffs = hediffs.Where(IsSignificantHediff).ToList();

            if (importantHediffs.Count == 0)
            {
                Widgets.Label(new Rect(0, curY, width, Text.LineHeight), "No significant health conditions.");
                curY += Text.LineHeight + 2f;
                return;
            }

            importantHediffs = importantHediffs.OrderByDescending(GetSeverity).ToList();

            int numColumns = 2;
            float columnWidth = (width - 10f) / numColumns;
            float iconSize = Text.LineHeight;
            float padding = 4f;

            int numRows = (importantHediffs.Count + numColumns - 1) / numColumns;

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numColumns; col++)
                {
                    int index = row + col * numRows;
                    if (index >= importantHediffs.Count)
                        continue;

                    var hediff = importantHediffs[index];

                    float x = col * (columnWidth + padding);
                    float y = curY;

                    Rect iconRect = new Rect(x, y, iconSize, iconSize);
                    Texture2D icon = GetHediffIcon(hediff);
                    if (icon != null)
                    {
                        GUI.DrawTexture(iconRect, icon);
                    }

                    string hediffLabel = hediff.LabelCap;
                    string bodyPartLabel = hediff.Part != null ? hediff.Part.LabelCap : "Whole body";
                    string label = $"{hediffLabel} ({bodyPartLabel})";

                    Rect labelRect = new Rect(iconRect.xMax + padding, y, columnWidth - iconSize - padding * 2, Text.LineHeight);

                    Color textColor = GetSeverityColor(hediff);
                    GUI.color = textColor;
                    Widgets.Label(labelRect, label.Truncate(labelRect.width));
                    GUI.color = Color.white;

                    string tooltip = GetTooltip(hediff);
                    Rect tooltipRect = new Rect(x, y, columnWidth, Text.LineHeight);
                    TooltipHandler.TipRegion(tooltipRect, tooltip);
                }
                curY += Text.LineHeight + padding;
            }

            curY += 5f;
        }

        private static bool IsSignificantHediff(Hediff hediff)
        {
            if (hediff is Hediff_Injury injury)
            {
                return injury.Severity >= 5f || injury.IsPermanent();
            }
            else if (hediff.def.makesSickThought)
            {
                return true;
            }
            else if (hediff.def.IsAddiction)
            {
                return true;
            }
            else if (hediff is Hediff_MissingPart missingPart && missingPart.IsFresh)
            {
                return true;
            }
            else if (hediff.CurStage != null && hediff.CurStage.lifeThreatening)
            {
                return true;
            }

            return false;
        }

        private static float GetSeverity(Hediff hediff)
        {
            return hediff.Severity;
        }

        private static Texture2D GetHediffIcon(Hediff hediff)
        {
            if (hediff is Hediff_Injury)
            {
                return ContentFinder<Texture2D>.Get("UI/Icons/Health/Injury", false);
            }
            else if (hediff.def.IsAddiction)
            {
                return ContentFinder<Texture2D>.Get("UI/Icons/Health/Addiction", false);
            }
            else if (hediff.def.makesSickThought)
            {
                return ContentFinder<Texture2D>.Get("UI/Icons/Health/Illness", false);
            }
            else
            {
                return ContentFinder<Texture2D>.Get("UI/Icons/Health/Condition", false);
            }
        }

        private static Color GetSeverityColor(Hediff hediff)
        {
            if (hediff.Severity >= 0.75f)
            {
                return Color.red; // Critical
            }
            else if (hediff.Severity >= 0.5f)
            {
                return new Color(1f, 0.5f, 0f); // Serious (Orange)
            }
            else if (hediff.Severity >= 0.25f)
            {
                return Color.yellow; // Moderate
            }
            else
            {
                return Color.white; // Minor
            }
        }

        private static string GetTooltip(Hediff hediff)
        {
            string tooltip = hediff.LabelCap;

            if (!string.IsNullOrEmpty(hediff.def.description))
            {
                tooltip += "\n\n" + hediff.def.description;
            }

            if (!string.IsNullOrEmpty(hediff.TipStringExtra))
            {
                tooltip += "\n\n" + hediff.TipStringExtra;
            }

            return tooltip;
        }
    }
}
