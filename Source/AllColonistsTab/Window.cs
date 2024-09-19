using RimWorld;
using UnityEngine;
using Verse;
using System.Linq;
using System.Collections.Generic;

namespace AllColonistsTab
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
                .ThenBy(p => p.PawnInstance.Name.ToStringFull);

            deadPawns = PawnRetriever
                .GetDeadPlayerPawns()
                .Select(p => new PawnWithStats(p, tilePosition))
                .OrderBy(p => p.PawnInstance.Corpse == null ? -1 : p.JoinTick);
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

                // Born
                var birthdayDate = selectedPawnWithStats.BirthdayDate;
                DrawLabelValuePair(rightPanelRect.x, ref curY, labelWidth, valueWidth, "Born:", birthdayDate);

                // Joined at
                var joinDate = selectedPawnWithStats.JoinDate;
                DrawLabelValuePair(rightPanelRect.x, ref curY, labelWidth, valueWidth, "Joined at:", joinDate);

                // Died or Biological age
                if (selectedPawnWithStats.PawnInstance.Dead)
                {
                    string deathInfo = $"{selectedPawnWithStats.DeathDate} (aged {selectedPawnWithStats.BiologicalAge})";
                    DrawLabelValuePair(rightPanelRect.x, ref curY, labelWidth, valueWidth, "Died:", deathInfo);
                }
                else
                {
                    DrawLabelValuePair(rightPanelRect.x, ref curY, labelWidth, valueWidth, "Biological age:", selectedPawnWithStats.BiologicalAge);
                }

                // Time as colonist
                DrawLabelValuePair(rightPanelRect.x, ref curY, labelWidth, valueWidth, "Time as a colonist:", selectedPawnWithStats.TimeAsColonist);

                curY += 5f;
                Widgets.DrawLineHorizontal(rightPanelRect.x, curY, contentWidth);
                curY += 5f;

                float scrollContentHeight = 2000f;
                Rect scrollRect = new Rect(rightPanelRect.x, curY, contentWidth + 16f, rightPanelRect.height - (curY - rightPanelRect.y));
                Rect viewRect = new Rect(0, 0, contentWidth, scrollContentHeight);

                Widgets.BeginScrollView(scrollRect, ref pawnDetailsScrollPosition, viewRect);

                float curYScroll = 0f;

                // Skills
                Widgets.Label(new Rect(0, curYScroll, viewRect.width, Text.LineHeight), "Skills:");
                curYScroll += Text.LineHeight + 2f;

                var skills = selectedPawnWithStats.PawnInstance.skills;
                DrawSkills(ref curYScroll, viewRect.width, skills);

                // Traits
                curYScroll += 5f;
                Widgets.DrawLineHorizontal(0, curYScroll, viewRect.width);
                curYScroll += 5f;

                Widgets.Label(new Rect(0, curYScroll, viewRect.width, Text.LineHeight), "Traits:");
                curYScroll += Text.LineHeight + 2f;

                var traits = selectedPawnWithStats.PawnInstance.story?.traits;
                DrawTraits(ref curYScroll, viewRect.width, traits);

                // Backstories
                curYScroll += 5f;
                Widgets.DrawLineHorizontal(0, curYScroll, viewRect.width);
                curYScroll += 5f;

                Widgets.Label(new Rect(0, curYScroll, viewRect.width, Text.LineHeight), "Backstories:");
                curYScroll += Text.LineHeight + 2f;

                var story = selectedPawnWithStats.PawnInstance.story;
                DrawBackstories(ref curYScroll, viewRect.width, story);

                // Health Details
                curYScroll += 5f;
                Widgets.DrawLineHorizontal(0, curYScroll, viewRect.width);
                curYScroll += 5f;

                Widgets.Label(new Rect(0, curYScroll, viewRect.width, Text.LineHeight), "Health:");
                curYScroll += Text.LineHeight + 2f;

                var health = selectedPawnWithStats.PawnInstance.health;
                DrawHealthDetails(ref curYScroll, viewRect.width, health);

                // Relationships
                curYScroll += 5f;
                Widgets.DrawLineHorizontal(0, curYScroll, viewRect.width);
                curYScroll += 5f;

                Widgets.Label(new Rect(0, curYScroll, viewRect.width, Text.LineHeight), "Relationships:");
                curYScroll += Text.LineHeight + 2f;

                var relations = selectedPawnWithStats.PawnInstance.relations;
                DrawSocialDetails(ref curYScroll, viewRect.width, relations);

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

        private void DrawSkills(ref float curY, float width, Pawn_SkillTracker skills)
        {
            if (skills == null || skills.skills == null)
            {
                Widgets.Label(new Rect(0, curY, width, Text.LineHeight), "Skills information not available.");
                curY += Text.LineHeight + 2f;
                return;
            }

            float columnWidth = width / 3f;
            int skillsPerColumn = Mathf.CeilToInt(skills.skills.Count / 3f);

            for (int i = 0; i < skillsPerColumn; i++)
            {
                for (int col = 0; col < 3; col++)
                {
                    int skillIndex = i + col * skillsPerColumn;
                    if (skillIndex >= skills.skills.Count) break;

                    var skill = skills.skills[skillIndex];
                    if (skill == null || skill.def == null)
                    {
                        continue;
                    }

                    string skillLabel = skill.def.label.CapitalizeFirst();
                    int skillLevel = skill.Level;

                    Color skillColor = skillLevel switch
                    {
                        >= 12 => Color.green,
                        >= 6 => Color.yellow,
                        _ => Color.red
                    };

                    Rect skillRect = new Rect(col * columnWidth, curY, columnWidth, Text.LineHeight);

                    Widgets.Label(new Rect(skillRect.x, skillRect.y, columnWidth * 0.6f, skillRect.height), $"{skillLabel}: ");

                    GUI.color = skillColor;
                    Widgets.Label(new Rect(skillRect.x + columnWidth * 0.6f, skillRect.y, columnWidth * 0.4f, skillRect.height), skillLevel.ToString());
                    GUI.color = Color.white;
                }
                curY += Text.LineHeight + 2f;
            }
        }

        private void DrawTraits(ref float curY, float width, TraitSet traits)
        {
            if (traits == null || traits.allTraits == null)
            {
                Widgets.Label(new Rect(0, curY, width, Text.LineHeight), "Traits information not available.");
                curY += Text.LineHeight + 2f;
                return;
            }

            float startX = 0f;
            float curX = startX;
            float maxWidth = width;

            foreach (var trait in traits.allTraits)
            {
                if (trait == null)
                {
                    continue;
                }

                string traitLabel = trait.LabelCap;
                Vector2 size = Text.CalcSize(traitLabel) + new Vector2(10f, 2f);

                if (curX + size.x > maxWidth)
                {
                    curX = startX;
                    curY += Text.LineHeight + 2f;
                }

                Rect traitRect = new Rect(curX, curY, size.x, Text.LineHeight + 2f);

                Widgets.DrawBoxSolid(traitRect, new Color(0.2f, 0.2f, 0.2f));

                Widgets.DrawHighlightIfMouseover(traitRect);

                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(traitRect, traitLabel);
                Text.Anchor = TextAnchor.UpperLeft;

                TooltipHandler.TipRegion(traitRect, trait.TipString(selectedPawnWithStats.PawnInstance));

                curX += size.x + 5f;
            }

            curY += Text.LineHeight + 4f;
        }

        private void DrawBackstories(ref float curY, float width, Pawn_StoryTracker story)
        {
            if (story == null)
            {
                Widgets.Label(new Rect(0, curY, width, Text.LineHeight), "Backstory information not available.");
                curY += Text.LineHeight + 2f;
                return;
            }

            float labelWidth = 120f;
            float padding = 4f; //

            string childhood = story.Childhood != null ? story.Childhood.TitleFor(selectedPawnWithStats.PawnInstance.gender).CapitalizeFirst() : "Unknown";
            Rect labelRect = new Rect(0, curY, labelWidth, Text.LineHeight + padding * 2f);
            Widgets.Label(labelRect, "Childhood:");

            Vector2 textSize = Text.CalcSize(childhood);
            Rect valueRect = new Rect(labelRect.xMax + 10f, curY, textSize.x + padding * 2f, Text.LineHeight + padding * 2f);

            Widgets.DrawBoxSolid(valueRect, new Color(0.2f, 0.2f, 0.2f));

            Widgets.DrawHighlightIfMouseover(valueRect);

            Rect textRect = new Rect(valueRect.x + padding, valueRect.y + padding, textSize.x, Text.LineHeight);

            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(textRect, childhood);
            Text.Anchor = TextAnchor.UpperLeft;

            if (story.Childhood != null)
            {
                TooltipHandler.TipRegion(valueRect, story.Childhood.FullDescriptionFor(selectedPawnWithStats.PawnInstance));
            }

            curY += valueRect.height + 2f;

            string adulthood = story.Adulthood != null ? story.Adulthood.TitleFor(selectedPawnWithStats.PawnInstance.gender).CapitalizeFirst() : "Unknown";
            labelRect = new Rect(0, curY, labelWidth, Text.LineHeight + padding * 2f);
            Widgets.Label(labelRect, "Adulthood:");

            textSize = Text.CalcSize(adulthood);
            valueRect = new Rect(labelRect.xMax + 10f, curY, textSize.x + padding * 2f, Text.LineHeight + padding * 2f);

            Widgets.DrawBoxSolid(valueRect, new Color(0.2f, 0.2f, 0.2f));

            Widgets.DrawHighlightIfMouseover(valueRect);

            textRect = new Rect(valueRect.x + padding, valueRect.y + padding, textSize.x, Text.LineHeight);

            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(textRect, adulthood);
            Text.Anchor = TextAnchor.UpperLeft;

            if (story.Adulthood != null)
            {
                TooltipHandler.TipRegion(valueRect, story.Adulthood.FullDescriptionFor(selectedPawnWithStats.PawnInstance));
            }

            curY += valueRect.height + 2f;
        }

        private void DrawHealthDetails(ref float curY, float width, Pawn_HealthTracker health)
        {
            if (health == null || health.hediffSet == null || health.hediffSet.hediffs == null)
            {
                Widgets.Label(new Rect(0, curY, width, Text.LineHeight), "Health information not available.");
                curY += Text.LineHeight + 2f;
                return;
            }

            var hediffs = health.hediffSet.hediffs;
            if (hediffs.Count > 0)
            {
                foreach (var hediff in hediffs)
                {
                    if (hediff == null)
                    {
                        continue;
                    }

                    string hediffLabel = hediff.LabelCap;
                    string bodyPartLabel = hediff.Part != null ? hediff.Part.LabelCap : "Whole body";
                    string severityLabel = hediff.SeverityLabel;

                    string label = $"{hediffLabel} ({bodyPartLabel})";
                    if (!string.IsNullOrEmpty(severityLabel))
                    {
                        label += $" - Severity: {severityLabel}";
                    }

                    Widgets.Label(new Rect(0, curY, width, Text.LineHeight), label);
                    curY += Text.LineHeight + 2f;
                }
            }
            else
            {
                Widgets.Label(new Rect(0, curY, width, Text.LineHeight), "No health conditions.");
                curY += Text.LineHeight + 2f;
            }
        }

        private void DrawSocialDetails(ref float curY, float width, Pawn_RelationsTracker relations)
        {
            if (relations == null || relations.DirectRelations == null)
            {
                Widgets.Label(new Rect(0, curY, width, Text.LineHeight), "Relationships information not available.");
                curY += Text.LineHeight + 2f;
                return;
            }

            var directRelations = relations.DirectRelations;
            float padding = 4f;

            if (directRelations != null && directRelations.Count > 0)
            {
                foreach (var rel in directRelations)
                {
                    if (rel == null || rel.def == null || rel.otherPawn == null)
                    {
                        continue;
                    }

                    string relationType = rel.def.GetGenderSpecificLabel(rel.otherPawn)?.CapitalizeFirst() ?? "Unknown relation";
                    string otherPawnName = rel.otherPawn.Name?.ToStringFull ?? "Unnamed pawn";

                    Vector2 relationLabelSize = Text.CalcSize(relationType + ": ");
                    Vector2 textSize = Text.CalcSize(otherPawnName);

                    float rowHeight = Text.LineHeight + padding * 2f;

                    Rect relationLabelRect = new Rect(0, curY, relationLabelSize.x, rowHeight);

                    Text.Anchor = TextAnchor.MiddleLeft;
                    Widgets.Label(relationLabelRect, relationType + ": ");
                    Text.Anchor = TextAnchor.UpperLeft;

                    Rect nameRect = new Rect(relationLabelRect.xMax, curY, textSize.x + padding * 2f, rowHeight);

                    Widgets.DrawBoxSolid(nameRect, new Color(0.2f, 0.2f, 0.2f));

                    Widgets.DrawHighlightIfMouseover(nameRect);

                    if (Widgets.ButtonInvisible(nameRect))
                    {
                        selectedPawnWithStats = new PawnWithStats(rel.otherPawn, Find.WorldGrid.LongLatOf(Find.CurrentMap.Tile));

                        pawnDetailsScrollPosition = Vector2.zero;
                        pawnListScrollPosition = Vector2.zero;
                    }

                    Rect textRect = new Rect(nameRect.x + padding, nameRect.y + padding, textSize.x, Text.LineHeight);

                    Text.Anchor = TextAnchor.MiddleLeft;
                    Widgets.Label(textRect, otherPawnName);
                    Text.Anchor = TextAnchor.UpperLeft;

                    curY += rowHeight + 2f;
                }
            }
            else
            {
                Widgets.Label(new Rect(0, curY, width, Text.LineHeight), "No relationships.");
                curY += Text.LineHeight + 2f;
            }
        }

        private void DrawLabelValuePair(float x, ref float curY, float labelWidth, float valueWidth, string label, string value)
        {
            Widgets.Label(new Rect(x, curY, labelWidth, Text.LineHeight), label);
            Widgets.Label(new Rect(x + labelWidth + 10f, curY, valueWidth, Text.LineHeight), value);
            curY += Text.LineHeight + 2f;
        }
    }
}
