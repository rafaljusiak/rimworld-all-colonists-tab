using RimWorld;
using UnityEngine;
using Verse;

namespace PawnTimeline
{

    internal class Window : MainTabWindow
    {
        private readonly Listing_Standard listing = new Listing_Standard();

        public override void DoWindowContents(Rect inRect)
        {
            listing.Begin(inRect);
            listing.Label("Here we go");
        }
    }
}
