using System;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions
{
    public class IncidentWorker_FaultyGenerator : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out int num);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            bool result;
            if (!(parms.target is Map map))
            {
                result = false;
            }

            else if ((from wo in Find.WorldObjects.Sites
                      where wo is Site && wo.parts.Select(x => x.def) == SiteDefOfReconAndDiscovery.RD_QuakesQuest
                      select wo).Count<WorldObject>() > 0)
            {
                result = false;
            }
            else
            {
                if (TileFinder.TryFindNewSiteTile(out int tile))
                {
                    Site site = (Site)WorldObjectMaker.MakeWorldObject(SiteDefOfReconAndDiscovery.RD_AdventureDestroyThing);
                    site.Tile = tile;
                    site.AddPart(new SitePart(site, SiteDefOfReconAndDiscovery.RD_QuakesQuest,
                        SiteDefOfReconAndDiscovery.RD_QuakesQuest.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null)));
                    SitePart faultyGenerator = new SitePart(site, SiteDefOfReconAndDiscovery.RD_SitePart_FaultyGenerator,
                        SiteDefOfReconAndDiscovery.RD_SitePart_FaultyGenerator.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null))
                    {
                        hidden = true
                    };
                    site.parts.Add(faultyGenerator);
                    site.GetComponent<QuestComp_DestroyThing>().StartQuest(ThingDefOf.GeothermalGenerator);
                    site.GetComponent<QuestComp_DestroyThing>().gameConditionCaused = GameConditionDef.Named("RD_Tremors");
                    site.GetComponent<QuestComp_DestroyThing>().worldTileAffected = map.Tile;
                    if (Rand.Value < 0.05f)
                    {
                        SitePart scatteredTreasure = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredTreasure, SiteDefOfReconAndDiscovery.RD_ScatteredTreasure.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null))
                        {
                            hidden = true
                        };
                        site.parts.Add(scatteredTreasure);
                    }
                    if (Rand.Value < 0.1f)
                    {
                        SitePart scatteredManhunters = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredManhunters, SiteDefOfReconAndDiscovery.RD_ScatteredManhunters.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null))
                        {
                            hidden = true
                        };
                        site.parts.Add(scatteredManhunters);
                    }
                    if (Rand.Value < 0.1f)
                    {
                        SitePart mechanoidForces = new SitePart(site, SiteDefOfReconAndDiscovery.RD_MechanoidForces, SiteDefOfReconAndDiscovery.RD_MechanoidForces.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null))
                        {
                            hidden = true
                        };
                        site.parts.Add(mechanoidForces);
                    }
                    if (Rand.Value < 0.05f)
                    {
                        SitePart enemyRaidOnArrival = new SitePart(site, SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival, SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null))
                        {
                            hidden = true
                        };
                        site.parts.Add(enemyRaidOnArrival);
                    }
                    int num = 30;
                    GameCondition gameCondition = GameConditionMaker.MakeCondition(GameConditionDef.Named("RD_Tremors"), 60000 * num);
                    map.gameConditionManager.RegisterCondition(gameCondition);
                    site.GetComponent<TimeoutComp>().StartTimeout(num * 60000);
                    base.SendStandardLetter(parms, site);
                    Find.WorldObjects.Add(site);
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }
    }
}

