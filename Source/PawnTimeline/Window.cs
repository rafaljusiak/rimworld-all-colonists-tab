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
            int tileIndex = Find.CurrentMap.Tile;
            Vector2 tilePosition = Find.WorldGrid.LongLatOf(tileIndex);

            alivePawns = PawnRetriever.GetAlivePlayerPawns().Select(p => new PawnWithStats(p, tilePosition)).OrderBy(p => p.JoinTick);
            deadPawns = PawnRetriever.GetDeadPlayerPawns().Select(p => new PawnWithStats(p, tilePosition)).OrderBy(
                p => p.PawnInstance.Corpse == null ? -1 : p.JoinTick  // TODO: don't look at join tick -- look at the now-time_death-time_as_colonist (or smth)
            );
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
            DrawPawnGraphic(rightPanelRect);
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
                if (selectedPawnWithStats.PawnInstance.Dead && selectedPawnWithStats.PawnInstance.Corpse == null)
                {
                    listing.Label($"No corpse found for {selectedPawnWithStats.PawnInstance.Name} -- cannot display details.");
                    listing.End();
                    return;
                }

                string pawnName = selectedPawnWithStats.PawnInstance.Name.ToStringFull;
                listing.Label($"Details for: {pawnName}");

                var birthdayDate = selectedPawnWithStats.BirthdayDate;
                listing.Label($"Born: {birthdayDate}");

                var joinDate = selectedPawnWithStats.JoinDate;
                listing.Label($"Joined at: {joinDate}");

                if (selectedPawnWithStats.PawnInstance.Dead)
                {
                    listing.Label($"Died: {selectedPawnWithStats.DeathDate} (aged {selectedPawnWithStats.BiologicalAge})");
                }
                else
                {
                    listing.Label($"Biological age: {selectedPawnWithStats.BiologicalAge}");
                }

                listing.Label($"Time as a colonist: {selectedPawnWithStats.TimeAsColonist}");

                listing.GapLine();
                listing.Label("More details:");

                listing.Gap();
            }
            else
            {
                listing.Label("Select a pawn to see details.");
            }
            listing.End();
        }

        private void DrawPawnGraphic(Rect rightPanelRect)
        {
            if (selectedPawnWithStats != null)
            {
                if (selectedPawnWithStats.PawnInstance.Dead && selectedPawnWithStats.PawnInstance.Corpse == null)
                {
                    return;
                }

                float iconSize = 100f;
                Rect pawnRect = new Rect(rightPanelRect.xMax - iconSize - 10, 0, iconSize, iconSize);

                Vector2 pawnTextureSize = new Vector2(iconSize, iconSize);
                Vector3 pawnTextureCameraOffset = new Vector3(0f, 0f, 0.3f);

                var portrait = PortraitsCache.Get(selectedPawnWithStats.PawnInstance, pawnTextureSize, Rot4.South, pawnTextureCameraOffset, 1.28205f);
                GUI.DrawTexture(pawnRect, portrait);
            }
        }
    }
}
