using RimWorld;
using UnityEngine;
using Verse;

namespace AllColonistsTab.Window
{
    public static class TraitDrawer
    {
        public static void DrawTraits(ref float curY, float width, TraitSet traits, Pawn pawn)
        {
            if (traits == null || traits.allTraits == null)
            {
                Widgets.Label(new Rect(0, curY, width, Text.LineHeight), "Traits information not available.");
                curY += Text.LineHeight + 2f;
                return;
            }

            float startX = 0f;
            float curX = startX;
            float maxWidth = width;
            float padding = 4f;

            foreach (var trait in traits.allTraits)
            {
                if (trait == null)
                {
                    continue;
                }

                string traitLabel = trait.LabelCap;
                Vector2 size = Text.CalcSize(traitLabel) + new Vector2(padding * 2f, padding * 2f);

                if (curX + size.x > maxWidth)
                {
                    curX = startX;
                    curY += Text.LineHeight + padding * 2f;
                }

                Rect traitRect = new Rect(curX, curY, size.x, Text.LineHeight + padding * 2f);

                Widgets.DrawBoxSolid(traitRect, new Color(0.2f, 0.2f, 0.2f));
                Widgets.DrawHighlightIfMouseover(traitRect);

                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(traitRect, traitLabel);
                Text.Anchor = TextAnchor.UpperLeft;

                TooltipHandler.TipRegion(traitRect, trait.TipString(pawn));

                curX += size.x + padding;
            }

            curY += Text.LineHeight + padding * 4f;
        }
    }
}
