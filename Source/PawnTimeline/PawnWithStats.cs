using RimWorld;
using UnityEngine;
using Verse;

using System;

namespace PawnTimeline
{
    public class PawnWithStats
    {
        public Pawn PawnInstance;

        public PawnWithStats(Pawn p)
        {
            PawnInstance = p;
        }

        public string JoinDate
        {
            get
            {
                int tileIndex = Find.CurrentMap.Tile;
                Vector2 tilePosition = Find.WorldGrid.LongLatOf(tileIndex);

                try
                {
                    if (PawnInstance.Dead)
                    {
                        if (PawnInstance.Corpse == null)
                        {
                            return "Join date unknown (no corpse)";
                        }

                        int deathTick = GenTicks.TicksAbs - PawnInstance.Corpse.Age;
                        int timeAsColonist = PawnInstance.records.GetAsInt(RecordDefOf.TimeAsColonistOrColonyAnimal);
                        int joinTick = deathTick - timeAsColonist;

                        string joinDate = GenDate.DateFullStringAt(joinTick, tilePosition);
                        return joinDate;
                    }
                    else
                    {
                        int timeAsColonist = PawnInstance.records.GetAsInt(RecordDefOf.TimeAsColonistOrColonyAnimal);
                        int joinTick = GenTicks.TicksAbs - timeAsColonist;
                        string joinDate = GenDate.DateFullStringAt(joinTick, tilePosition);
                        return joinDate;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error retrieving join date for pawn {PawnInstance.Name}: {ex.Message}");
                    return "Join date unknown";
                }
            }
        }
    }
}
