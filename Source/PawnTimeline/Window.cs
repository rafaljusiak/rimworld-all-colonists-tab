using RimWorld;
using UnityEngine;
using Verse;
using System;

namespace PawnTimeline
{
    internal class Window : MainTabWindow
    {
        private readonly Listing_Standard listing = new Listing_Standard();

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;

            listing.Begin(inRect);
            listing.Label("Alive pawns:");

            var pawns = PawnRetriever.GetAlivePlayerPawns();
            foreach (var pawn in pawns)
            {
                var joinDate = GetJoinDate(pawn);
                listing.Label(pawn.Name.ToStringFull + " - " + joinDate);
            }

            var deadPawns = PawnRetriever.GetDeadPlayerPawns();
            listing.Label("Dead pawns:");
            foreach (var deadPawn in deadPawns)
            {
                var joinDate = GetJoinDate(deadPawn);
                listing.Label(deadPawn.Name.ToStringFull + " - " + joinDate);
            }
            listing.End();
        }

        private static string GetJoinDate(Pawn pawn)
        {
            try
            {
                if (pawn.Dead)
                {
                    if (pawn.Corpse == null)
                    {
                        return "Join date unknown (no corpse)";
                    }

                    int deathTick = pawn.Corpse.timeOfDeath;
                    int timeAsColonist = pawn.records.GetAsInt(RecordDefOf.TimeAsColonistOrColonyAnimal);
                    int joinTick = deathTick - timeAsColonist;

                    string joinDate = GenDate.DateFullStringAt(joinTick, Find.WorldGrid.LongLatOf(pawn.Map.Tile));
                    return joinDate;
                }
                else
                {
                    int timeAsColonist = pawn.records.GetAsInt(RecordDefOf.TimeAsColonistOrColonyAnimal);
                    int joinTick = GenTicks.TicksAbs - timeAsColonist;
                    string joinDate = GenDate.DateFullStringAt(joinTick, Find.WorldGrid.LongLatOf(pawn.Map.Tile));
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
