using RimWorld;
using UnityEngine;
using Verse;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PawnTimeline
{
    internal class Window : MainTabWindow
    {
        private readonly Listing_Standard listing = new Listing_Standard();
        private Vector2 scrollPosition;
        private PawnWithStats selectedPawnWithStats;

        private IEnumerable<PawnWithStats> alivePawns;
        private IEnumerable<PawnWithStats> deadPawns;

        public override void PreOpen()
        {
            base.PreOpen();
            alivePawns = PawnRetriever.GetAlivePlayerPawns().Select(p => new PawnWithStats(p));
            deadPawns = PawnRetriever.GetDeadPlayerPawns().Select(p => new PawnWithStats(p));
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;

            float leftPanelWidth = inRect.width * 0.33f;
            float rightPanelWidth = inRect.width * 0.67f;

            Rect leftPanelRect = new Rect(0, 0, leftPanelWidth, inRect.height);
            Rect rightPanelRect = new Rect(leftPanelWidth, 0, rightPanelWidth, inRect.height);

            DrawPawns(leftPanelRect, inRect, leftPanelWidth);
            DrawPawnDetails(rightPanelRect);
        }

        private void DrawPawns(Rect leftPanelRect, Rect inRect, float leftPanelWidth)
        {
            Rect scrollRect = new Rect(0, 0, leftPanelWidth, inRect.height - 50);
            Rect viewRect = new Rect(0, 0, leftPanelWidth - 16, 1000);

            Widgets.BeginScrollView(scrollRect, ref scrollPosition, viewRect);

            listing.Begin(viewRect);
            listing.Label("Alive pawns:");

            foreach (var p in alivePawns)
            {
                if (Widgets.ButtonText(listing.GetRect(Text.LineHeight), p.PawnInstance.Name.ToStringFull))
                {
                    selectedPawnWithStats = p;
                }
            }

            listing.GapLine();
            listing.Label("Dead pawns:");

            foreach (var p in deadPawns)
            {
                if (Widgets.ButtonText(listing.GetRect(Text.LineHeight), p.PawnInstance.Name.ToStringFull))
                {
                    selectedPawnWithStats = p;
                }
            }

            listing.GapLine();
            listing.Gap();

            listing.End();
            Widgets.EndScrollView();
        }

        private void DrawPawnDetails(Rect rightPanelRect)
        {
            listing.Begin(rightPanelRect);
            if (selectedPawnWithStats != null)
            {
                string pawnName = selectedPawnWithStats.PawnInstance.Name.ToStringFull;
                listing.Label($"Details for: {pawnName}");

                var joinDate = GetJoinDate(selectedPawnWithStats.PawnInstance);
                listing.Label($"Join Date: {joinDate}");

                listing.GapLine();
                listing.Label($"More details about {pawnName}:");

                listing.Gap();
            }
            else
            {
                listing.Label("Select a pawn to see details.");
            }
            listing.End();
        }

        private static string GetJoinDate(Pawn pawn)
        {
            int tileIndex = Find.CurrentMap.Tile;
            Vector2 tilePosition = Find.WorldGrid.LongLatOf(tileIndex);

            try
            {
                if (pawn.Dead)
                {
                    if (pawn.Corpse == null)
                    {
                        return "Join date unknown (no corpse)";
                    }

                    int deathTick = GenTicks.TicksAbs - pawn.Corpse.Age;
                    int timeAsColonist = pawn.records.GetAsInt(RecordDefOf.TimeAsColonistOrColonyAnimal);
                    int joinTick = deathTick - timeAsColonist;

                    string joinDate = GenDate.DateFullStringAt(joinTick, tilePosition);
                    return joinDate;
                }
                else
                {
                    int timeAsColonist = pawn.records.GetAsInt(RecordDefOf.TimeAsColonistOrColonyAnimal);
                    int joinTick = GenTicks.TicksAbs - timeAsColonist;
                    string joinDate = GenDate.DateFullStringAt(joinTick, tilePosition);
                    return joinDate;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error retrieving join date for pawn {pawn.Name}: {ex.Message}");
                return "Join date unknown";
            }
        }
    }
}
