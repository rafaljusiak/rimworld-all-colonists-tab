using RimWorld;
using UnityEngine;
using Verse;

namespace AllColonistsTab.Window
{
    public static class BackstoryDrawer
    {
        public static void DrawBackstories(ref float curY, float width, Pawn_StoryTracker story, Pawn pawn)
        {
            if (story == null)
            {
                Widgets.Label(new Rect(0, curY, width, Text.LineHeight), "Backstory information not available.");
                curY += Text.LineHeight + 2f;
                return;
            }

            float labelWidth = 120f;
            float padding = 4f;

            // Childhood
            string childhood = story.Childhood != null ? story.Childhood.TitleFor(pawn.gender).CapitalizeFirst() : "Unknown";
            Rect labelRect = new Rect(0, curY, labelWidth, Text.LineHeight + padding * 2f);
            Widgets.Label(labelRect, "Childhood:");

            Vector2 textSize = Text.CalcSize(childhood);
            Rect valueRect = new Rect(labelRect.xMax + 10f, curY, textSize.x + padding * 2f, Text.LineHeight + padding * 2f);

            Widgets.DrawBoxSolid(valueRect, new Color(0.2f, 0.2f, 0.2f));
            Widgets.DrawHighlightIfMouseover(valueRect);

            Rect textRect = new Rect(valueRect.x + padding, valueRect.y + padding, textSize.x, Text.LineHeight);
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(textRect, childhood);
            Text.Anchor = TextAnchor.UpperLeft;

            if (story.Childhood != null)
            {
                TooltipHandler.TipRegion(valueRect, story.Childhood.FullDescriptionFor(pawn));
            }

            curY += valueRect.height + 2f;

            // Adulthood
            string adulthood = story.Adulthood != null ? story.Adulthood.TitleFor(pawn.gender).CapitalizeFirst() : "Unknown";
            labelRect = new Rect(0, curY, labelWidth, Text.LineHeight + padding * 2f);
            Widgets.Label(labelRect, "Adulthood:");

            textSize = Text.CalcSize(adulthood);
            valueRect = new Rect(labelRect.xMax + 10f, curY, textSize.x + padding * 2f, Text.LineHeight + padding * 2f);

            Widgets.DrawBoxSolid(valueRect, new Color(0.2f, 0.2f, 0.2f));
            Widgets.DrawHighlightIfMouseover(valueRect);

            textRect = new Rect(valueRect.x + padding, valueRect.y + padding, textSize.x, Text.LineHeight);
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(textRect, adulthood);
            Text.Anchor = TextAnchor.UpperLeft;

            if (story.Adulthood != null)
            {
                TooltipHandler.TipRegion(valueRect, story.Adulthood.FullDescriptionFor(pawn));
            }

            curY += valueRect.height + 2f;
        }
    }
}
