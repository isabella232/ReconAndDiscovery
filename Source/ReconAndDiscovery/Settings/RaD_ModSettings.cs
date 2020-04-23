using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ExpandedIncidents.Settings
{
    // Token: 0x0200000D RID: 13
    internal class RaD_ModSettings : ModSettings
    {
        internal static float RaidEnemyQuestBaseChance = 0.0f;
        internal static float DiscoveredStargateBaseChance = 0.0f;
        internal static float RaidStargateBaseChance = 1.9f;
        internal static float MalevolentAIBaseChance = 16f;
        internal static float RaidTeleporterBaseChance = 20.0f;
        internal static float IncidentTremorsBaseChance = 0.2f;
        internal static float IncidentRadiationBaseChance = 0.2f;

        public void ChangeDef()
        {
            List<IncidentDef> list = DefDatabase<IncidentDef>.AllDefs.ToList();
            foreach (IncidentDef incidentDef in list)
            {
                switch (incidentDef.defName)
                {
                    case "RD_RaidEnemyQuest":
                        incidentDef.baseChance = RaidEnemyQuestBaseChance;
                        break;
                    case "RD_DiscoveredStargate":
                        incidentDef.baseChance = DiscoveredStargateBaseChance;
                        break;
                    case "RD_RaidStargate":
                        incidentDef.baseChance = RaidStargateBaseChance;
                        break;
                    case "RD_MalevolentAI":
                        incidentDef.baseChance = MalevolentAIBaseChance;
                        break;
                    case "RD_RaidTeleporter":
                        incidentDef.baseChance = RaidTeleporterBaseChance;
                        break;
                    case "RD_IncidentTremors":
                        incidentDef.baseChance = IncidentTremorsBaseChance;
                        break;
                    case "RD_IncidentRadiation":
                        incidentDef.baseChance = IncidentRadiationBaseChance;
                        break;
                }
            }
        }

        public static void ChangeDefPost()
        {
            List<IncidentDef> list = DefDatabase<IncidentDef>.AllDefs.ToList();
            foreach (IncidentDef incidentDef in list)
            {
                switch (incidentDef.defName)
                {
                    case "RD_RaidEnemyQuest":
                        incidentDef.baseChance = RaidEnemyQuestBaseChance;
                        break;
                    case "RD_DiscoveredStargate":
                        incidentDef.baseChance = DiscoveredStargateBaseChance;
                        break;
                    case "RD_RaidStargate":
                        incidentDef.baseChance = RaidStargateBaseChance;
                        break;
                    case "RD_MalevolentAI":
                        incidentDef.baseChance = MalevolentAIBaseChance;
                        break;
                    case "RD_RaidTeleporter":
                        incidentDef.baseChance = RaidTeleporterBaseChance;
                        break;
                    case "RD_IncidentTremors":
                        incidentDef.baseChance = IncidentTremorsBaseChance;
                        break;
                    case "RD_IncidentRadiation":
                        incidentDef.baseChance = IncidentRadiationBaseChance;
                        break;
                }
            }
        }

        // Token: 0x06000024 RID: 36 RVA: 0x0000345C File Offset: 0x0000165C
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref RaidEnemyQuestBaseChance, "RaidEnemyQuestBaseChance", 0.0f, false);
            Scribe_Values.Look(ref DiscoveredStargateBaseChance, "DiscoveredStargateBaseChance", 0.0f, false);
            Scribe_Values.Look(ref RaidStargateBaseChance, "RaidStargateBaseChance", 1.9f, false);
            Scribe_Values.Look(ref MalevolentAIBaseChance, "MalevolentAIBaseChance", 16f, false);
            Scribe_Values.Look(ref RaidTeleporterBaseChance, "RaidTeleporterBaseChance", 20.0f, false);
            Scribe_Values.Look(ref IncidentTremorsBaseChance, "IncidentTremorsBaseChance", 0.2f, false);
            Scribe_Values.Look(ref IncidentRadiationBaseChance, "IncidentRadiationBaseChance", 0.2f, false);
        }


    }
}
