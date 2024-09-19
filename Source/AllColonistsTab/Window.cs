using RimWorld;
using UnityEngine;
using Verse;
using System.Linq;
using System.Collections.Generic;

namespace AllColonistsTab
{
    internal class Window : MainTabWindow
    {
        private readonly Listing_Standard listing = new Listing_Standard();
        private Vector2 scrollPosition;
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

            DrawPawns(leftPanelRect, inRect, leftPanelWidth);
            DrawPawnDetails(rightPanelRect);
            DrawPawnGraphic(rightPanelRect);
        }

        private void DrawPawns(Rect leftPanelRect, Rect inRect, float leftPanelWidth)
        {
            Rect scrollRect = new Rect(0, 0, leftPanelWidth, inRect.height - 50);
            Rect viewRect = new Rect(0, 0, leftPanelWidth - 16, 1000);

            Widgets.BeginScrollView(scrollRect, ref scrollPosition, viewRect);

            listing.Begin(viewRect);
            listing.Label("Alive colonists:");

            foreach (var p in alivePawns)
            {
                if (Widgets.ButtonText(listing.GetRect(Text.LineHeight), p.PawnInstance.Name.ToStringFull))
                {
                    selectedPawnWithStats = p;
                }
            }

            listing.GapLine();
            listing.Label("Dead colonists:");

            foreach (var p in deadPawns)
            {
                if (Widgets.ButtonText(listing.GetRect(Text.LineHeight), p.PawnInstance.Name.ToStringFull))
                {
                    selectedPawnWithStats = p;
                }
            }

            listing.GapLine();
            listing.Gap();

            listing.End();
            Widgets.EndScrollView();
        }

        private void DrawPawnDetails(Rect rightPanelRect)
        {
            listing.Begin(rightPanelRect);
            if (selectedPawnWithStats != null)
            {
                if (selectedPawnWithStats.PawnInstance.Dead && selectedPawnWithStats.PawnInstance.Corpse == null)
                {
                    listing.Label($"No corpse found for {selectedPawnWithStats.PawnInstance.Name} -- cannot display details.");
                    listing.End();
                    return;
                }

                Text.Font = GameFont.Medium;
                string pawnName = selectedPawnWithStats.PawnInstance.Name.ToStringFull;
                listing.Label(pawnName);
                Text.Font = GameFont.Small;

                var birthdayDate = selectedPawnWithStats.BirthdayDate;
                listing.Label($"Born: {birthdayDate}");

                var joinDate = selectedPawnWithStats.JoinDate;
                listing.Label($"Joined at: {joinDate}");

                if (selectedPawnWithStats.PawnInstance.Dead)
                {
                    listing.Label($"Died: {selectedPawnWithStats.DeathDate} (aged {selectedPawnWithStats.BiologicalAge})");
                }
                else
                {
                    listing.Label($"Biological age: {selectedPawnWithStats.BiologicalAge}");
                }

                listing.Label($"Time as a colonist: {selectedPawnWithStats.TimeAsColonist}");

                listing.GapLine();
                listing.Gap();

                // Skills
                listing.Label("Skills:");

                var skills = selectedPawnWithStats.PawnInstance.skills;
                if (skills != null)
                {
                    DrawSkills(rightPanelRect, skills);
                }
                else
                {
                    listing.Label("Skills information not available.");
                }

                // Traits
                listing.GapLine();
                listing.Label("Traits:");

                var traits = selectedPawnWithStats.PawnInstance.story?.traits;
                if (traits != null)
                {
                    DrawTraits(traits);
                }
                else
                {
                    listing.Label("Traits information not available.");
                }

                // Backstories
                listing.GapLine();
                listing.Label("Backstories:");

                var story = selectedPawnWithStats.PawnInstance.story;
                if (story != null)
                {
                    DrawBackstories(story);
                }
                else
                {
                    listing.Label("Backstory information not available.");
                }

                // Health Details
                listing.GapLine();
                listing.Label("Health:");

                var health = selectedPawnWithStats.PawnInstance.health;
                if (health != null)
                {
                    DrawHealthDetails(health);
                }
                else
                {
                    listing.Label("Health information not available.");
                }

                // Social Details
                listing.GapLine();
                listing.Label("Social:");

                var social = selectedPawnWithStats.PawnInstance.relations;
                if (social != null)
                {
                    DrawSocialDetails(social);
                }
                else
                {
                    listing.Label("Social information not available.");
                }
            }
            else
            {
                listing.Label("Select a pawn to see details.");
            }
            listing.End();
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
                }
            }
        }

        private void DrawSkills(Rect rightPanelRect, Pawn_SkillTracker skills)
        {
            float columnWidth = rightPanelRect.width / 3f;
            int skillsPerColumn = Mathf.CeilToInt(skills.skills.Count / 3f);

            for (int i = 0; i < skillsPerColumn; i++)
            {
                for (int col = 0; col < 3; col++)
                {
                    int skillIndex = i + col * skillsPerColumn;
                    if (skillIndex >= skills.skills.Count) break;

                    var skill = skills.skills[skillIndex];
                    string skillLabel = skill.def.label.CapitalizeFirst();
                    int skillLevel = skill.Level;

                    Color skillColor = skillLevel switch
                    {
                        >= 12 => Color.green,
                        >= 6 => Color.yellow,
                        _ => Color.red
                    };

                    Rect skillRect = new Rect(col * columnWidth, listing.CurHeight, columnWidth, Text.LineHeight);

                    Widgets.Label(new Rect(skillRect.x, skillRect.y, columnWidth * 0.6f, skillRect.height), $"{skillLabel}: ");

                    GUI.color = skillColor;
                    Widgets.Label(new Rect(skillRect.x + columnWidth * 0.6f, skillRect.y, columnWidth * 0.4f, skillRect.height), skillLevel.ToString());

                    GUI.color = Color.white;
                }
                listing.Gap(Text.LineHeight);
            }
        }

        private void DrawTraits(TraitSet traits)
        {
            foreach (var trait in traits.allTraits)
            {
                string traitLabel = trait.LabelCap;
                listing.Label(traitLabel);
            }
        }

        private void DrawBackstories(Pawn_StoryTracker story)
        {
            string title = story.Title ?? "No title";
            string childhood = story.Childhood?.TitleFor(selectedPawnWithStats.PawnInstance.gender).CapitalizeFirst() ?? "Unknown";
            string adulthood = story.Adulthood?.TitleFor(selectedPawnWithStats.PawnInstance.gender).CapitalizeFirst() ?? "Unknown";

            listing.Label($"Childhood: {childhood}");
            listing.Label($"Adulthood: {adulthood}");
        }

        private void DrawHealthDetails(Pawn_HealthTracker health)
        {
            var hediffs = health.hediffSet.hediffs;
            if (hediffs.Count > 0)
            {
                foreach (var hediff in hediffs)
                {
                    string hediffLabel = hediff.LabelCap;
                    listing.Label(hediffLabel);
                }
            }
            else
            {
                listing.Label("No health conditions.");
            }
        }

        private void DrawSocialDetails(Pawn_RelationsTracker relations)
        {
            var directRelations = relations.DirectRelations;

            if (directRelations != null && directRelations.Count > 0)
            {
                foreach (var rel in directRelations)
                {
                    string relationLabel = $"{rel.def.GetGenderSpecificLabel(rel.otherPawn)}: {rel.otherPawn.Name.ToStringFull}";
                    listing.Label(relationLabel);
                }
            }
            else
            {
                listing.Label("No social relationships.");
            }
        }
    }
}
