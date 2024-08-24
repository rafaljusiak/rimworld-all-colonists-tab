using RimWorld;
using UnityEngine;
using Verse;

namespace PawnTimeline
{
    internal class Window : MainTabWindow
    {
        private readonly Listing_Standard listing = new Listing_Standard();

        public override void DoWindowContents(Rect inRect)
        {
            listing.Begin(inRect);
            listing.Label("Here we go");

            var pawns = PawnRetriever.GetPlayerPawns();
            foreach (var pawn in pawns)
            {
                var joinDate = GetJoinDate(pawn);
                listing.Label(pawn.Name.ToStringFull + " - " + joinDate);
            }

            listing.End();
        }

        private static string GetJoinDate(Pawn pawn)
        {
            int timeAsColonist = pawn.records.GetAsInt(RecordDefOf.TimeAsColonistOrColonyAnimal);
            int joinTick = GenTicks.TicksAbs - timeAsColonist;
            string joinDate = GenDate.DateFullStringAt(joinTick, Find.WorldGrid.LongLatOf(pawn.Map.Tile));
            return joinDate;
        }
    }
}
