using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions
{
	public class IncidentWorker_OsirisCasket : IncidentWorker
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			int num;
			return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out num);
		}

		private bool CanFindPsychic(Map map, out Pawn pawn)
		{
			pawn = null;
			IEnumerable<Pawn> source = from p in map.mapPawns.FreeColonistsSpawned
			where p.RaceProps.Humanlike && !p.Faction.HostileTo(Faction.OfPlayer) && p.story.traits.HasTrait(TraitDef.Named("PsychicSensitivity"))
			select p;
			bool result;
			if (source.Count<Pawn>() == 0)
			{
				result = false;
			}
			else
			{
				pawn = source.RandomElement<Pawn>();
				result = true;
			}
			return result;
		}

		private bool GetHasGoodStoryConditions(Map map)
		{
			Pawn pawn;
			return map != null && this.CanFindPsychic(map, out pawn);
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = parms.target as Map;
			bool result;
			if (this.GetHasGoodStoryConditions(map))
			{
				Pawn pawn;
				if (!this.CanFindPsychic(map, out pawn))
				{
					result = false;
				}
				else
				{
					int randomInRange = IncidentWorker_OsirisCasket.TimeoutDaysRange.RandomInRange;

                    int tile;
                    if (TileFinder.TryFindNewSiteTile(out tile))
                    {

                        Site site = (Site)WorldObjectMaker.MakeWorldObject(SiteDefOfReconAndDiscovery.RD_Adventure);
                        site.Tile = tile;
                        Faction faction = Faction.OfInsects;
                        site.SetFaction(faction);
                        site.AddPart(new SitePart(site, SiteDefOfReconAndDiscovery.RD_AbandonedCastle,
SiteDefOfReconAndDiscovery.RD_AbandonedCastle.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction)));
                        IEnumerable<PowerNet> source = from net in map.powerNetManager.AllNetsListForReading
                                                       where net.hasPowerSource
                                                       select net;
                        if (source.Count<PowerNet>() > 0)
                        {
                            SitePart osirisCasket = new SitePart(site, SiteDefOfReconAndDiscovery.RD_OsirisCasket, SiteDefOfReconAndDiscovery.RD_OsirisCasket.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
                            osirisCasket.hidden = true;
                            site.parts.Add(osirisCasket);
                        }
                        if (Rand.Value < 0.15f)
                        {
                            SitePart weatherSat = new SitePart(site, SiteDefOfReconAndDiscovery.RD_WeatherSat, SiteDefOfReconAndDiscovery.RD_WeatherSat.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
                            weatherSat.hidden = true;
                            site.parts.Add(weatherSat);
                        }
                        site.GetComponent<TimeoutComp>().StartTimeout(randomInRange * 60000);
                        if (Rand.Value < 0.25f)
                        {
                            SitePart scatteredManhunters = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredManhunters, SiteDefOfReconAndDiscovery.RD_ScatteredManhunters.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
                            scatteredManhunters.hidden = true;
                            site.parts.Add(scatteredManhunters);
                        }
                        if (Rand.Value < 0.1f)
                        {
                            SitePart scatteredTreasure = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredTreasure, SiteDefOfReconAndDiscovery.RD_ScatteredTreasure.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
                            scatteredTreasure.hidden = true;
                            site.parts.Add(scatteredTreasure);
                        }
                        if (Rand.Value < 1f)
                        {
                            SitePart enemyRaidOnArrival = new SitePart(site, SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival, SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
                            enemyRaidOnArrival.hidden = true;
                            site.parts.Add(enemyRaidOnArrival);
                        }
                        if (Rand.Value < 0.9f)
                        {
                            SitePart enemyRaidOnArrival = new SitePart(site, SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival, SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
                            enemyRaidOnArrival.hidden = true;
                            site.parts.Add(enemyRaidOnArrival);
                        }
                        if (Rand.Value < 0.6f)
                        {
                            SitePart enemyRaidOnArrival = new SitePart(site, SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival, SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
                            enemyRaidOnArrival.hidden = true;
                            site.parts.Add(enemyRaidOnArrival);
                        }
                        Find.WorldObjects.Add(site);
                        QueuedIncident qi = new QueuedIncident(new FiringIncident(IncidentDef.Named("PsychicDrone"), null, parms), Find.TickManager.TicksGame + 1);
                        Find.Storyteller.incidentQueue.Add(qi);
                        Find.LetterStack.ReceiveLetter("RD_PsychicMessage".Translate(), 
GrammarResolverSimpleStringExtensions.Formatted(Translator
                    .Translate("RD_ReceivedVisionBattle"), NamedArgumentUtility.Named(pawn, "PAWN"))
                    .AdjustedFor(pawn, "PAWN", true)
 //has received visions accompanying the drone, showing a battle and crying out for help. Others must have noticed, so the site will probably be dangerous.
, LetterDefOf.PositiveEvent, null);
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
			}
			else
			{
				result = false;
			}
			return result;
		}

		private static readonly IntRange TimeoutDaysRange = new IntRange(15, 25);
	}
}

