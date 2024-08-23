using System.Collections.Generic;

namespace PawnTimeline
{
    [Verse.StaticConstructorOnStartup]
    public static class PawnTimeline
    {
        static PawnTimeline()
        {
            Verse.Log.Message("Hello, World!");
        }
    }

    public static class PawnNameRetriever
    {
        public static List<string> GetPawnNames()
        {
            List<string> pawnNames = new List<string>();
            foreach (Verse.Map map in Verse.Find.Maps)
            {
                foreach (Verse.Pawn pawn in map.mapPawns.AllPawns)
                {
                    pawnNames.Add(pawn.Name.ToStringFull);
                }
            }

            return pawnNames;
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(Verse.Game), nameof(Verse.Game.LoadGame))]
    public static class LoadGamePatch
    {
        public static void Postfix()
        {
            Verse.Log.Message("PawnTimeline loaded");

            var pawnNames = PawnNameRetriever.GetPawnNames();
            foreach (var name in pawnNames)
            {
                Verse.Log.Message(name);
            }
        }
    }
}
