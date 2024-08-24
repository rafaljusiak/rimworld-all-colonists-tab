using System.Collections.Generic;
using Verse;
using RimWorld;

namespace PawnTimeline
{
    public static class PawnRetriever
    {
        public static List<Pawn> GetPlayerPawns()
        {
            List<Pawn> pawns = new List<Pawn>();

            foreach (Map map in Find.Maps)
            {
                if (map?.mapPawns?.AllPawns != null)
                {
                    foreach (Pawn pawn in map.mapPawns.AllPawns)
                    {
                        if (pawn?.Name != null && pawn.Faction == Faction.OfPlayer && pawn.def == ThingDefOf.Human)
                        {
                            pawns.Add(pawn);
                        }
                    }
                }
            }

            return pawns;
        }
    }
}
