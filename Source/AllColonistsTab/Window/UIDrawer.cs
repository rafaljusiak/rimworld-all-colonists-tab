using UnityEngine;
using Verse;

namespace AllColonistsTab.Window
{
    public static class UIDrawer
    {
        public static void DrawLabelValuePair(float x, ref float curY, float labelWidth, float valueWidth, string label, string value)
        {
            Widgets.Label(new Rect(x, curY, labelWidth, Text.LineHeight), label);
            Widgets.Label(new Rect(x + labelWidth + 10f, curY, valueWidth, Text.LineHeight), value);
            curY += Text.LineHeight + 2f;
        }

        public static void DrawSectionHeader(ref float curY, float width, string title)
        {
            Widgets.Label(new Rect(0, curY, width, Text.LineHeight), title);
            curY += Text.LineHeight + 2f;
        }

        public static void DrawSeparator(ref float curY, float width)
        {
            DrawSeparator(ref curY, 0f, width);
        }

        public static void DrawSeparator(ref float curY, float x, float width)
        {
            curY += 5f;
            Widgets.DrawLineHorizontal(x, curY, width);
            curY += 5f;
        }

        public static void DrawLabeledBox(Rect labelRect, string labelText, Rect valueRect, string valueText, string tooltip = null)
        {
            Widgets.Label(labelRect, labelText);

            Widgets.DrawBoxSolid(valueRect, new Color(0.2f, 0.2f, 0.2f));
            Widgets.DrawHighlightIfMouseover(valueRect);

            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(valueRect, valueText);
            Text.Anchor = TextAnchor.UpperLeft;

            if (!string.IsNullOrEmpty(tooltip))
            {
                TooltipHandler.TipRegion(valueRect, tooltip);
            }
        }
    }
}
