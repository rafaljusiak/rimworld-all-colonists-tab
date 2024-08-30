using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace PawnTimeline
{
    public static class PawnRetriever
    {
        public static IEnumerable<Pawn> GetAlivePlayerPawns()
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

        public static IEnumerable<Pawn> GetDeadPlayerPawns()
        {
            return Find.WorldPawns.AllPawnsDead.Where((Pawn p) => p.IsColonist);
        }
    }
}
