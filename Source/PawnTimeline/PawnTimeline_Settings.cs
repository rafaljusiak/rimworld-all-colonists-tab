using Verse;

namespace PawnTimeline
{

    public class PawnTimeline_Settings : ModSettings
    {
        public bool enableAnniversaries = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.enableAnniversaries, "enableAnniversaries", true);
        }
    }
}
