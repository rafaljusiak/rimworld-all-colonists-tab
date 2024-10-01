using RimWorld;
using UnityEngine;
using Verse;

using System;

namespace AllColonistsTab
{
    public class PawnWithStats
    {
        public Pawn PawnInstance;
        private Vector2 tilePosition;

        // Cached JoinTick
        public int JoinTick { get; private set; }

        public PawnWithStats(Pawn p, Vector2 tilePosition)
        {
            PawnInstance = p;
            this.tilePosition = tilePosition;

            // Calculate and store JoinTick
            JoinTick = CalculateJoinTick();
        }

        private int CalculateJoinTick()
        {
            try
            {
                int timeAsColonistTicks = PawnInstance.records.GetAsInt(RecordDefOf.TimeAsColonistOrColonyAnimal);

                if (timeAsColonistTicks <= 0)
                {
                    return 0;
                }

                int joinTick;

                if (PawnInstance.Dead)
                {
                    if (PawnInstance.Corpse == null)
                    {
                        return 0;
                    }

                    int deathTick = this.deathTick;
                    joinTick = deathTick - timeAsColonistTicks;
                }
                else
                {
                    // Use the tick count at the time of calculation
                    int currentTickAtCalculation = Find.TickManager.TicksAbs;
                    joinTick = currentTickAtCalculation - timeAsColonistTicks;
                }

                if (joinTick <= 0)
                {
                    return 0;
                }

                return joinTick;
            }
            catch (Exception ex)
            {
                Log.Error($"Error calculating join tick for pawn {PawnInstance.Name}: {ex.Message}");
                return 0;
            }
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
                    if (JoinTick > 0)
                    {
                        return GenDate.DateFullStringAt(JoinTick, tilePosition);
                    }
                    else
                    {
                        return "Join date unknown";
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
            get { return ConvertTicksToTimePeriod(timeAsColonistTicks); }
        }

        public string BiologicalAge
        {
            get { return PawnInstance.ageTracker.AgeBiologicalYears.ToString(); }
        }

        private int timeAsColonistTicks
        {
            get
            {
                return PawnInstance.records.GetAsInt(RecordDefOf.TimeAsColonistOrColonyAnimal);
            }
        }

        private long birthdayTick { get { return PawnInstance.ageTracker.BirthAbsTicks; } }
        private int deathTick
        {
            get
            {
                if (PawnInstance.Corpse != null)
                {
                    return GenTicks.TicksAbs - PawnInstance.Corpse.Age;
                }
                else
                {
                    // Estimate death tick if corpse is missing
                    return GenTicks.TicksAbs - timeAsColonistTicks;
                }
            }
        }

        private string ConvertTicksToTimePeriod(long ticks)
        {
            long years = ticks / GenDate.TicksPerYear;
            ticks %= GenDate.TicksPerYear;

            long months = ticks / GenDate.TicksPerTwelfth;
            ticks %= GenDate.TicksPerTwelfth;

            long days = ticks / GenDate.TicksPerDay;
            return $"{years} years, {months} months, {days} days";
        }
    }
}
