using UnityEngine;
using Verse;

namespace ExpandedIncidents.Settings
{
    // Token: 0x0200000E RID: 14
    internal class RaD_Mod : Mod
    {
        // Token: 0x0400003D RID: 61
        private static RaD_ModSettings settings;

        // Token: 0x0400003E RID: 62
        private Vector2 scrollPosition = Vector2.zero;

        // Token: 0x06000027 RID: 39 RVA: 0x00003737 File Offset: 0x00001937
        public RaD_Mod(ModContentPack content) : base(content)
        {
            settings = GetSettings<RaD_ModSettings>();
        }

        // Token: 0x06000028 RID: 40 RVA: 0x00003758 File Offset: 0x00001958
        public override string SettingsCategory()
        {
            return "Recon And Discovery";
        }

        // Token: 0x06000029 RID: 41 RVA: 0x0000376C File Offset: 0x0000196C
        private void ResetSettings()
        {
            RaD_ModSettings.RaidEnemyQuestBaseChance = 0.0f;
            RaD_ModSettings.DiscoveredStargateBaseChance = 0.0f;
            RaD_ModSettings.RaidStargateBaseChance = 1.9f;
            RaD_ModSettings.MalevolentAIBaseChance = 16f;
            RaD_ModSettings.RaidTeleporterBaseChance = 20.0f;
            RaD_ModSettings.IncidentTremorsBaseChance = 0.2f;
            RaD_ModSettings.IncidentRadiationBaseChance = 0.2f;
            settings.Write();
            settings.ChangeDef();
        }

        // Token: 0x0600002C RID: 44 RVA: 0x000039CC File Offset: 0x00001BCC
        public override void DoSettingsWindowContents(Rect rect)
        {
            settings.ChangeDef();
            var rect2 = new Rect(rect.x, rect.y, rect.width - 30f, rect.height - 10f);
            var listing_Standard = new Listing_Standard();
            Widgets.BeginScrollView(rect, ref scrollPosition, rect2);
            listing_Standard.Begin(rect2);
            listing_Standard.Gap(10f);
            var rect3 = listing_Standard.GetRect(Text.LineHeight);
            var flag = Widgets.ButtonText(rect3, "Reset Settings");
            if (flag)
            {
                ResetSettings();
            }

            listing_Standard.Gap(10f);

            var rect7 = listing_Standard.GetRect(Text.LineHeight);
            Widgets.Label(rect7, "RD_SettingHeader".Translate());
            listing_Standard.Gap(10f);
            var rect8 = listing_Standard.GetRect(Text.LineHeight);
            var rect9 = rect8.LeftHalf().Rounded();
            var rect10 = rect8.RightHalf().Rounded();
            var rect11 = rect9.LeftHalf().Rounded();
            var rect12 = rect9.RightHalf().Rounded();
            rect11.Overlaps(rect12);
            var rect13 = rect12.RightHalf().Rounded();
            Widgets.Label(rect11, "Enemy raid Quest");
            Widgets.Label(rect13, RaD_ModSettings.RaidEnemyQuestBaseChance.ToString());
            RaD_ModSettings.RaidEnemyQuestBaseChance = Widgets.HorizontalSlider(
                new Rect(rect10.xMin + rect10.height + 10f, rect10.y, rect10.width - ((rect10.height * 2f) + 20f),
                    rect10.height), RaD_ModSettings.RaidEnemyQuestBaseChance, 0f, 10f, true);
            listing_Standard.Gap(10f);
            var rect14 = listing_Standard.GetRect(Text.LineHeight);
            var rect15 = rect14.LeftHalf().Rounded();
            var rect16 = rect14.RightHalf().Rounded();
            var rect17 = rect15.LeftHalf().Rounded();
            var rect18 = rect15.RightHalf().Rounded();
            rect17.Overlaps(rect18);
            var rect19 = rect18.RightHalf().Rounded();
            Widgets.Label(rect17, "Discovered stargate");
            Widgets.Label(rect19, RaD_ModSettings.DiscoveredStargateBaseChance.ToString());
            RaD_ModSettings.DiscoveredStargateBaseChance = Widgets.HorizontalSlider(
                new Rect(rect16.xMin + rect16.height + 10f, rect16.y, rect16.width - ((rect16.height * 2f) + 20f),
                    rect16.height), RaD_ModSettings.DiscoveredStargateBaseChance, 0f, 10f, true);
            listing_Standard.Gap(10f);
            var rect20 = listing_Standard.GetRect(Text.LineHeight);
            var rect21 = rect20.LeftHalf().Rounded();
            var rect22 = rect20.RightHalf().Rounded();
            var rect23 = rect21.LeftHalf().Rounded();
            var rect24 = rect21.RightHalf().Rounded();
            rect23.Overlaps(rect24);
            var rect25 = rect24.RightHalf().Rounded();
            Widgets.Label(rect23, "Stargate raid");
            Widgets.Label(rect25, RaD_ModSettings.RaidStargateBaseChance.ToString());
            RaD_ModSettings.RaidStargateBaseChance = Widgets.HorizontalSlider(
                new Rect(rect22.xMin + rect22.height + 10f, rect22.y, rect22.width - ((rect22.height * 2f) + 20f),
                    rect22.height), RaD_ModSettings.RaidStargateBaseChance, 0f, 10f, true);
            listing_Standard.Gap(10f);
            var rect26 = listing_Standard.GetRect(Text.LineHeight);
            var rect27 = rect26.LeftHalf().Rounded();
            var rect28 = rect26.RightHalf().Rounded();
            var rect29 = rect27.LeftHalf().Rounded();
            var rect30 = rect27.RightHalf().Rounded();
            rect29.Overlaps(rect30);
            var rect31 = rect30.RightHalf().Rounded();
            Widgets.Label(rect29, "Malevolent AI");
            Widgets.Label(rect31, RaD_ModSettings.MalevolentAIBaseChance.ToString());
            RaD_ModSettings.MalevolentAIBaseChance = Widgets.HorizontalSlider(
                new Rect(rect28.xMin + rect28.height + 10f, rect28.y, rect28.width - ((rect28.height * 2f) + 20f),
                    rect28.height), RaD_ModSettings.MalevolentAIBaseChance, 0f, 20f, true);
            listing_Standard.Gap(10f);
            var rect32 = listing_Standard.GetRect(Text.LineHeight);
            var rect33 = rect32.LeftHalf().Rounded();
            var rect34 = rect32.RightHalf().Rounded();
            var rect35 = rect33.LeftHalf().Rounded();
            var rect36 = rect33.RightHalf().Rounded();
            rect35.Overlaps(rect36);
            var rect37 = rect36.RightHalf().Rounded();
            Widgets.Label(rect35, "Teleporter hack");
            Widgets.Label(rect37, RaD_ModSettings.RaidTeleporterBaseChance.ToString());
            RaD_ModSettings.RaidTeleporterBaseChance = Widgets.HorizontalSlider(
                new Rect(rect34.xMin + rect34.height + 10f, rect34.y, rect34.width - ((rect34.height * 2f) + 20f),
                    rect34.height), RaD_ModSettings.RaidTeleporterBaseChance, 0f, 20f, true);
            listing_Standard.Gap(10f);
            var rect38 = listing_Standard.GetRect(Text.LineHeight);
            var rect39 = rect38.LeftHalf().Rounded();
            var rect40 = rect38.RightHalf().Rounded();
            var rect41 = rect39.LeftHalf().Rounded();
            var rect42 = rect39.RightHalf().Rounded();
            rect41.Overlaps(rect42);
            var rect43 = rect42.RightHalf().Rounded();
            Widgets.Label(rect41, "Tremors");
            Widgets.Label(rect43, RaD_ModSettings.IncidentTremorsBaseChance.ToString());
            RaD_ModSettings.IncidentTremorsBaseChance = Widgets.HorizontalSlider(
                new Rect(rect40.xMin + rect40.height + 10f, rect40.y, rect40.width - ((rect40.height * 2f) + 20f),
                    rect40.height), RaD_ModSettings.IncidentTremorsBaseChance, 0f, 10f, true);
            listing_Standard.Gap(10f);
            var rect44 = listing_Standard.GetRect(Text.LineHeight);
            var rect45 = rect44.LeftHalf().Rounded();
            var rect46 = rect44.RightHalf().Rounded();
            var rect47 = rect45.LeftHalf().Rounded();
            var rect48 = rect45.RightHalf().Rounded();
            rect47.Overlaps(rect48);
            var rect49 = rect48.RightHalf().Rounded();
            Widgets.Label(rect47, "Radiation");
            Widgets.Label(rect49, RaD_ModSettings.IncidentRadiationBaseChance.ToString());
            RaD_ModSettings.IncidentRadiationBaseChance = Widgets.HorizontalSlider(
                new Rect(rect46.xMin + rect46.height + 10f, rect46.y, rect46.width - ((rect46.height * 2f) + 20f),
                    rect46.height), RaD_ModSettings.IncidentRadiationBaseChance, 0f, 10f, true);
            listing_Standard.End();
            Widgets.EndScrollView();
            settings.Write();
            settings.ChangeDef();
        }
    }
}