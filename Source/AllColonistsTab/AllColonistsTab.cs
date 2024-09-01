using HarmonyLib;
using Verse;

namespace AllColonistsTab
{

    public class AllColonistsTab : Mod
    {
        public AllColonistsTab(ModContentPack content) : base(content)
        {
            Harmony harmony = new Harmony("rafaljusiak.allcoloniststab");
            harmony.PatchAll();
        }
    }
}
