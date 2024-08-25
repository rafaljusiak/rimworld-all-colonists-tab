using HarmonyLib;
using Verse;

namespace PawnTimeline
{

    public class PawnTimeline : Mod
    {
        private readonly PawnTimeline_Settings settings;

        public PawnTimeline(ModContentPack content) : base(content)
        {
            Harmony harmony = new Harmony("rafaljusiak.pawntimeline");
            harmony.PatchAll();

            settings = GetSettings<PawnTimeline_Settings>();
        }
    }
}
