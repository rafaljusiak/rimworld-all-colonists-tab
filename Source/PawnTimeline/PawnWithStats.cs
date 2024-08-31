using RimWorld;
using UnityEngine;
using Verse;

using System;

namespace PawnTimeline
{
    public class PawnWithStats
    {
        public Pawn PawnInstance;
        private Vector2 tilePosition;

        public PawnWithStats(Pawn p, Vector2 tilePosition)
        {
            PawnInstance = p;
            this.tilePosition = tilePosition;
        }

        public string BirthdayDate
        {
            get
            {
                try
                {
                    return GenDate.DateFullStringAt(birthdayTick, tilePosition);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error retrieving birthday date for pawn {PawnInstance.Name}: {ex.Message}");
                    return "Birthday date unknown";
                }
            }
        }

        public string JoinDate
        {
            get
            {
                try
                {
                    if (PawnInstance.Dead)
                    {
                        if (PawnInstance.Corpse == null)
                        {
                            return "Join date unknown (no corpse)";
                        }

                        int joinTick = deathTick - timeAsColonist;
                        return GenDate.DateFullStringAt(joinTick, tilePosition);
                    }
                    else
                    {
                        return GenDate.DateFullStringAt(joinTick, tilePosition);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error retrieving join date for pawn {PawnInstance.Name}: {ex.Message}");
                    return "Join date unknown";
                }
            }
        }

        public string DeathDate
        {
            get
            {
                if (PawnInstance.Dead)
                    return GenDate.DateFullStringAt(deathTick, tilePosition);

                return null;
            }
        }

        public string TimeAsColonist
        {
            get { return convertTicksToTimePeriod(timeAsColonist); }
        }

        public string BiologicalAge
        {
            get { return PawnInstance.ageTracker.AgeBiologicalYears.ToString(); }
        }

        public int JoinTick { get { return PawnInstance.Dead ? deathTick - timeAsColonist : joinTick; } }

        private int joinTick { get { return GenTicks.TicksAbs - PawnInstance.records.GetAsInt(RecordDefOf.TimeAsColonistOrColonyAnimal); } }
        private int timeAsColonist { get { return PawnInstance.records.GetAsInt(RecordDefOf.TimeAsColonistOrColonyAnimal); } }
        private long birthdayTick { get { return PawnInstance.ageTracker.BirthAbsTicks; } }
        private int deathTick { get { return GenTicks.TicksAbs - PawnInstance.Corpse.Age; } }

        private string convertTicksToTimePeriod(long ticks)
        {
            long years = ticks / GenDate.TicksPerYear;
            ticks %= GenDate.TicksPerYear;

            int ticksPerMonth = 15 * GenDate.TicksPerDay;
            long months = ticks / (15 * GenDate.TicksPerDay);
            ticks %= ticksPerMonth;

            long days = ticks / GenDate.TicksPerDay;
            return $"{years} years, {months} months, {days} days";
        }
    }
}
