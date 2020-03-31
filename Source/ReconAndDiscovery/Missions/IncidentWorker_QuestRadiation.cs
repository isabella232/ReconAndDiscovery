using System;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions
{
	public class IncidentWorker_QuestRadiation : IncidentWorker
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			int num;
			return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out num);
		}
         
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = parms.target as Map;
			bool result;
			if (map == null)
			{
				result = false;
			}
			else if ((from wo in Find.WorldObjects.AllWorldObjects
			where wo is Site && (wo as Site).parts.Select(x => x.def) == SiteDefOfReconAndDiscovery.RD_QuakesQuest
			select wo).Count<WorldObject>() > 0)
			{
				result = false;
			}
			else
			{
                int tile;
                if (TileFinder.TryFindNewSiteTile(out tile))
                {
                    Site site = (Site)WorldObjectMaker.MakeWorldObject(SiteDefOfReconAndDiscovery.RD_AdventureThingCounter);
                    site.Tile = tile;
                    site.AddPart(new SitePart(site, SiteDefOfReconAndDiscovery.RD_SiteRadiationQuest,
SiteDefOfReconAndDiscovery.RD_SiteRadiationQuest.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null)));
                    SitePart radioactiveDust = new SitePart(site, SiteDefOfReconAndDiscovery.RD_SitePart_RadioactiveDust, SiteDefOfReconAndDiscovery.RD_SitePart_RadioactiveDust.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null));
                    radioactiveDust.hidden = true;
                    site.parts.Add(radioactiveDust);
                    QuestComp_CountThings component = site.GetComponent<QuestComp_CountThings>();
                    component.targetNumber = 200;
                    component.ticksTarget = 60000;
                    component.ticksHeld = 0;
                    component.worldTileAffected = map.Tile;
                    component.gameConditionCaused = GameConditionDef.Named("RD_Radiation");
                    component.StartQuest(ThingDef.Named("Plant_Psychoid"));
                    if (Rand.Value < 0.1f)
                    {
                        SitePart scatteredTreasure = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredTreasure, SiteDefOfReconAndDiscovery.RD_ScatteredTreasure.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null));
                        scatteredTreasure.hidden = true;
                        site.parts.Add(scatteredTreasure);
                    }
                    if (Rand.Value < 0.05f)
                    {
                        SitePart scatteredManhunters = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredManhunters, SiteDefOfReconAndDiscovery.RD_ScatteredManhunters.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null));
                        scatteredManhunters.hidden = true;
                        site.parts.Add(scatteredManhunters);
                    }
                    if (Rand.Value < 0.05f)
                    {
                        SitePart mechanoidForces = new SitePart(site, SiteDefOfReconAndDiscovery.RD_MechanoidForces, SiteDefOfReconAndDiscovery.RD_MechanoidForces.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null));
                        mechanoidForces.hidden = true;
                        site.parts.Add(mechanoidForces);
                    }
                    if (Rand.Value < 0.05f)
                    {
                        SitePart enemyRaidOnArrival = new SitePart(site, SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival, SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, null));
                        enemyRaidOnArrival.hidden = true;
                        site.parts.Add(enemyRaidOnArrival);
                    }

                    if (Find.World.tileTemperatures.GetSeasonalTemp(site.Tile) < 10f || Find.World.tileTemperatures.GetSeasonalTemp(site.Tile) > 40f)
                    {
                        result = false;
                    }
                    else
                    {
                        int num = 30;
                        GameCondition gameCondition = GameConditionMaker.MakeCondition(GameConditionDef.Named("RD_Radiation"), 60000 * num);
                        map.gameConditionManager.RegisterCondition(gameCondition);
                        site.GetComponent<TimeoutComp>().StartTimeout(num * 60000);
                        base.SendStandardLetter(parms, site);
                        Find.WorldObjects.Add(site);
                        result = true;
                    }
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

