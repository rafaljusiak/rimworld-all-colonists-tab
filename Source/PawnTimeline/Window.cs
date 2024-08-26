using RimWorld;
using UnityEngine;
using Verse;
using PawnTimeline.DebugUtils;
using System.Linq;

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
            var firstPawnLog = LogObject.GetLogFor(deadPawns.First());
            Log.Message(firstPawnLog);

            foreach (var pawn in deadPawns)
            {
                listing.Label(pawn.Name.ToStringFull);
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
