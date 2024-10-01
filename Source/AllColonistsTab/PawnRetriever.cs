using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace AllColonistsTab
{
    public static class PawnRetriever
    {
        public static IEnumerable<Pawn> GetAlivePlayerPawns()
        {
            return PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists;
        }

        public static IEnumerable<Pawn> GetDeadPlayerPawns()
        {
            var deadWorldPawns = Find.WorldPawns.AllPawnsDead
                .Where(pawn => IsPlayerPawn(pawn));

            var deadMapPawns = Find.Maps
                .SelectMany(map => map.mapPawns.AllPawnsSpawned)
                .Where(pawn => pawn.Dead && IsPlayerPawn(pawn));

            return deadWorldPawns.Concat(deadMapPawns).Distinct();
        }

        private static bool IsPlayerPawn(Pawn pawn)
        {
            return pawn != null
                && pawn.def == ThingDefOf.Human
                && pawn.Faction == Faction.OfPlayer
                && !pawn.IsSlave
                && !pawn.IsPrisoner;
        }
    }
}
