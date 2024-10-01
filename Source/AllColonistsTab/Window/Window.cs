using RimWorld;
using UnityEngine;
using Verse;
using System.Linq;
using System.Collections.Generic;

namespace AllColonistsTab.Window
{
    internal class Window : MainTabWindow
    {
        private Vector2 pawnDetailsScrollPosition;
        private Vector2 pawnListScrollPosition;
        private PawnWithStats selectedPawnWithStats;

        private IEnumerable<PawnWithStats> alivePawns;
        private IEnumerable<PawnWithStats> deadPawns;

        public override void PreOpen()
        {
            base.PreOpen();
            int tileIndex = Find.CurrentMap.Tile;
            Vector2 tilePosition = Find.WorldGrid.LongLatOf(tileIndex);

            alivePawns = PawnRetriever
                .GetAlivePlayerPawns()
                .Select(p => new PawnWithStats(p, tilePosition))
                .OrderBy(p => p.JoinTick)
                .ThenBy(p => p.PawnInstance.Name.ToStringFull)
                .ToList();

            deadPawns = PawnRetriever
                .GetDeadPlayerPawns()
                .Select(p => new PawnWithStats(p, tilePosition))
                .OrderBy(p => p.PawnInstance.Corpse == null ? -1 : p.JoinTick)
                .ThenBy(p => p.PawnInstance.Name.ToStringFull)
                .ToList();
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;

            float leftPanelWidth = inRect.width * 0.33f;
            float rightPanelWidth = inRect.width * 0.67f;

            Rect leftPanelRect = new Rect(0, 0, leftPanelWidth, inRect.height);
            Rect rightPanelRect = new Rect(leftPanelWidth, 0, rightPanelWidth, inRect.height);

            DrawPawns(leftPanelRect);
            DrawPawnDetails(rightPanelRect);
            DrawPawnGraphic(rightPanelRect);
        }

        private void DrawPawns(Rect leftPanelRect)
        {
            float contentHeight = (alivePawns.Count() + deadPawns.Count() + 4) * (Text.LineHeight + 2);
            Rect viewRect = new Rect(0, 0, leftPanelRect.width - 16, contentHeight);

            Widgets.BeginScrollView(leftPanelRect, ref pawnListScrollPosition, viewRect);

            float curY = 0f;

            Widgets.Label(new Rect(0, curY, viewRect.width, Text.LineHeight), "Alive colonists:");
            curY += Text.LineHeight + 2f;

            foreach (var p in alivePawns)
            {
                if (Widgets.ButtonText(new Rect(0, curY, viewRect.width, Text.LineHeight), p.PawnInstance.Name.ToStringFull))
                {
                    selectedPawnWithStats = p;
                    pawnDetailsScrollPosition = Vector2.zero;
                }
                curY += Text.LineHeight + 2f;
            }

            curY += 5f;
            Widgets.DrawLineHorizontal(0, curY, viewRect.width);
            curY += 5f;

            Widgets.Label(new Rect(0, curY, viewRect.width, Text.LineHeight), "Dead colonists:");
            curY += Text.LineHeight + 2f;

            foreach (var p in deadPawns)
            {
                if (Widgets.ButtonText(new Rect(0, curY, viewRect.width, Text.LineHeight), p.PawnInstance.Name.ToStringFull))
                {
                    selectedPawnWithStats = p;
                    pawnDetailsScrollPosition = Vector2.zero;
                }
                curY += Text.LineHeight + 2f;
            }

            Widgets.EndScrollView();
        }

        private void DrawPawnDetails(Rect rightPanelRect)
        {
            float contentWidth = rightPanelRect.width - 16;
            float curY = rightPanelRect.y;

            if (selectedPawnWithStats != null)
            {
                if (selectedPawnWithStats.PawnInstance.Dead && selectedPawnWithStats.PawnInstance.Corpse == null)
                {
                    Widgets.Label(new Rect(rightPanelRect.x, curY, contentWidth, Text.LineHeight), $"No corpse found for {selectedPawnWithStats.PawnInstance.Name} -- cannot display details.");
                    return;
                }

                Text.Font = GameFont.Medium;
                string pawnName = selectedPawnWithStats.PawnInstance.Name.ToStringFull;
                Widgets.Label(new Rect(rightPanelRect.x, curY, contentWidth, Text.LineHeight), pawnName);
                curY += Text.LineHeight + 5f;
                Text.Font = GameFont.Small;

                float labelWidth = 120f;
                float valueWidth = contentWidth - labelWidth - 10f;

                var birthdayDate = selectedPawnWithStats.BirthdayDate;
                UIDrawer.DrawLabelValuePair(rightPanelRect.x, ref curY, labelWidth, valueWidth, "Born:", birthdayDate);

                var joinDate = selectedPawnWithStats.JoinDate;
                UIDrawer.DrawLabelValuePair(rightPanelRect.x, ref curY, labelWidth, valueWidth, "Joined at:", joinDate);

                if (selectedPawnWithStats.PawnInstance.Dead)
                {
                    string deathInfo = $"{selectedPawnWithStats.DeathDate} (aged {selectedPawnWithStats.BiologicalAge})";
                    UIDrawer.DrawLabelValuePair(rightPanelRect.x, ref curY, labelWidth, valueWidth, "Died:", deathInfo);
                }
                else
                {
                    UIDrawer.DrawLabelValuePair(rightPanelRect.x, ref curY, labelWidth, valueWidth, "Biological age:", selectedPawnWithStats.BiologicalAge);
                }

                UIDrawer.DrawLabelValuePair(rightPanelRect.x, ref curY, labelWidth, valueWidth, "Time as a colonist:", selectedPawnWithStats.TimeAsColonist);

                if (selectedPawnWithStats.PawnInstance.Faction != Faction.OfPlayer)
                {
                    string factionName = selectedPawnWithStats.PawnInstance.Faction?.Name ?? "No Faction";
                    UIDrawer.DrawLabelValuePair(rightPanelRect.x, ref curY, labelWidth, valueWidth, "Faction:", factionName);
                }

                UIDrawer.DrawSeparator(ref curY, rightPanelRect.x, contentWidth);

                float scrollContentHeight = 2000f;
                Rect scrollRect = new Rect(rightPanelRect.x, curY, contentWidth + 16f, rightPanelRect.height - (curY - rightPanelRect.y));
                Rect viewRect = new Rect(0, 0, contentWidth, scrollContentHeight);

                Widgets.BeginScrollView(scrollRect, ref pawnDetailsScrollPosition, viewRect);

                float curYScroll = 0f;

                // Skills
                UIDrawer.DrawSectionHeader(ref curYScroll, viewRect.width, "Skills:");
                SkillDrawer.DrawSkills(ref curYScroll, viewRect.width, selectedPawnWithStats.PawnInstance.skills);

                // Traits
                UIDrawer.DrawSeparator(ref curYScroll, viewRect.width);
                UIDrawer.DrawSectionHeader(ref curYScroll, viewRect.width, "Traits:");
                TraitDrawer.DrawTraits(ref curYScroll, viewRect.width, selectedPawnWithStats.PawnInstance.story?.traits, selectedPawnWithStats.PawnInstance);

                // Backstories
                UIDrawer.DrawSeparator(ref curYScroll, viewRect.width);
                UIDrawer.DrawSectionHeader(ref curYScroll, viewRect.width, "Backstories:");
                BackstoryDrawer.DrawBackstories(ref curYScroll, viewRect.width, selectedPawnWithStats.PawnInstance.story, selectedPawnWithStats.PawnInstance);

                // Health Details
                UIDrawer.DrawSeparator(ref curYScroll, viewRect.width);
                UIDrawer.DrawSectionHeader(ref curYScroll, viewRect.width, "Health:");
                HealthDetailsDrawer.DrawHealthDetails(ref curYScroll, viewRect.width, selectedPawnWithStats.PawnInstance);

                // Relationships
                UIDrawer.DrawSeparator(ref curYScroll, viewRect.width);
                UIDrawer.DrawSectionHeader(ref curYScroll, viewRect.width, "Relationships:");
                SocialDetailsDrawer.DrawSocialDetails(
                    ref curYScroll,
                    viewRect.width,
                    selectedPawnWithStats.PawnInstance.relations,
                    ref selectedPawnWithStats,
                    ref pawnDetailsScrollPosition,
                    ref pawnListScrollPosition
                );

                Widgets.EndScrollView();
            }
            else
            {
                Widgets.Label(new Rect(rightPanelRect.x, curY, contentWidth, Text.LineHeight), "Select a pawn to see details.");
                curY += Text.LineHeight + 2f;
            }
        }

        private void DrawPawnGraphic(Rect rightPanelRect)
        {
            if (selectedPawnWithStats != null)
            {
                if (selectedPawnWithStats.PawnInstance.Dead && selectedPawnWithStats.PawnInstance.Corpse == null)
                {
                    return;
                }

                float iconSize = 100f;
                Rect pawnRect = new Rect(rightPanelRect.xMax - iconSize - 10, 0, iconSize, iconSize);

                Vector2 pawnTextureSize = new Vector2(iconSize, iconSize);
                Vector3 pawnTextureCameraOffset = new Vector3(0f, 0f, 0.3f);

                var portrait = PortraitsCache.Get(selectedPawnWithStats.PawnInstance, pawnTextureSize, Rot4.South, pawnTextureCameraOffset, 1.28205f);
                GUI.DrawTexture(pawnRect, portrait);

                Rect buttonRect = new Rect(pawnRect.x, pawnRect.yMax + 5f, pawnRect.width, 24f);
                if (Widgets.ButtonText(buttonRect, "Focus"))
                {
                    CameraJumper.TryJumpAndSelect(selectedPawnWithStats.PawnInstance);
                    Find.WindowStack.TryRemove(this);
                }
            }
        }
    }
}
