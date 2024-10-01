using RimWorld;
using UnityEngine;
using Verse;

namespace AllColonistsTab.Window
{
    public static class SkillDrawer
    {
        public static void DrawSkills(ref float curY, float width, Pawn_SkillTracker skills)
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
    }
}
