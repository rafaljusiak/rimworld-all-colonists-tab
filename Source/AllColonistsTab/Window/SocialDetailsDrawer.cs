using RimWorld;
using UnityEngine;
using Verse;

namespace AllColonistsTab.Window
{
    public static class SocialDetailsDrawer
    {
        public static void DrawSocialDetails(
            ref float curY,
            float width,
            Pawn_RelationsTracker relations,
            ref PawnWithStats selectedPawnWithStats,
            ref Vector2 pawnDetailsScrollPosition,
            ref Vector2 pawnListScrollPosition)
        {
            if (relations == null || relations.DirectRelations == null)
            {
                Widgets.Label(new Rect(0, curY, width, Text.LineHeight), "Relationships information not available.");
                curY += Text.LineHeight + 2f;
                return;
            }

            var directRelations = relations.DirectRelations;
            float padding = 4f;

            if (directRelations != null && directRelations.Count > 0)
            {
                foreach (var rel in directRelations)
                {
                    if (rel == null || rel.def == null || rel.otherPawn == null)
                    {
                        continue;
                    }

                    string relationType = rel.def.GetGenderSpecificLabel(rel.otherPawn)?.CapitalizeFirst() ?? "Unknown relation";
                    string otherPawnName = rel.otherPawn.Name?.ToStringFull ?? "Unnamed pawn";

                    Vector2 relationLabelSize = Text.CalcSize(relationType + ": ");
                    Vector2 textSize = Text.CalcSize(otherPawnName);

                    float rowHeight = Text.LineHeight + padding * 2f;

                    Rect relationLabelRect = new Rect(0, curY, relationLabelSize.x, rowHeight);

                    Text.Anchor = TextAnchor.MiddleLeft;
                    Widgets.Label(relationLabelRect, relationType + ": ");
                    Text.Anchor = TextAnchor.UpperLeft;

                    Rect nameRect = new Rect(relationLabelRect.xMax, curY, textSize.x + padding * 2f, rowHeight);

                    Widgets.DrawBoxSolid(nameRect, new Color(0.2f, 0.2f, 0.2f));
                    Widgets.DrawHighlightIfMouseover(nameRect);

                    if (Widgets.ButtonInvisible(nameRect))
                    {
                        selectedPawnWithStats = new PawnWithStats(rel.otherPawn, Find.WorldGrid.LongLatOf(Find.CurrentMap.Tile));
                        pawnDetailsScrollPosition = Vector2.zero;
                        pawnListScrollPosition = Vector2.zero;
                    }

                    Rect textRect = new Rect(nameRect.x + padding, nameRect.y + padding, textSize.x, Text.LineHeight);

                    Text.Anchor = TextAnchor.MiddleLeft;
                    Widgets.Label(textRect, otherPawnName);
                    Text.Anchor = TextAnchor.UpperLeft;

                    curY += rowHeight + 2f;
                }
            }
            else
            {
                Widgets.Label(new Rect(0, curY, width, Text.LineHeight), "No relationships.");
                curY += Text.LineHeight + 2f;
            }
        }
    }
}
