using HarmonyLib;
using Verse;

namespace PawnTimeline
{

    public class PawnTimeline : Mod
    {
        public PawnTimeline(ModContentPack content) : base(content)
        {
            Harmony harmony = new Harmony("rafaljusiak.pawntimeline");
            harmony.PatchAll();
        }
    }
}
