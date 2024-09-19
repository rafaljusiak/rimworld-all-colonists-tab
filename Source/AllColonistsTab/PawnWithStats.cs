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
                        return GenDate.DateFullStringAt(JoinTick, tilePosition);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error retrieving join date for pawn {PawnInstance.Name}: {ex.Message}");
                    return "Join date unknown";
                }
            }
        }

        public int JoinTick
        {
            get
            {
                try
                {
                    int joinTick;
                    float timeAsColonistSeconds = PawnInstance.records.GetValue(RecordDefOf.TimeAsColonistOrColonyAnimal);

                    int timeAsColonistSecondsRounded = Mathf.RoundToInt(timeAsColonistSeconds);
                    int timeAsColonistTicks = timeAsColonistSecondsRounded * 60;

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
                        int currentTick = Find.TickManager.TicksGame;
                        joinTick = currentTick - timeAsColonistTicks;
                    }

                    if (joinTick < 0)
                    {
                        return 0;
                    }

                    return joinTick;
                }
                catch (Exception ex)
                {
                    Log.Error($"Error retrieving join tick for pawn {PawnInstance.Name}: {ex.Message}");
                    return 0;
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
