using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions
{
    public class IncidentWorker_OsirisCasket : IncidentWorker
    {
        private static readonly IntRange TimeoutDaysRange = new IntRange(15, 25);

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out _);
        }

        private bool CanFindPsychic(Map map, out Pawn pawn)
        {
            pawn = null;
            var source = from p in map.mapPawns.FreeColonistsSpawned
                where p.RaceProps.Humanlike && !p.Faction.HostileTo(Faction.OfPlayer) &&
                      p.story.traits.HasTrait(TraitDef.Named("PsychicSensitivity"))
                select p;
            bool result;
            if (!source.Any())
            {
                result = false;
            }
            else
            {
                pawn = source.RandomElement();
                result = true;
            }

            return result;
        }

        private bool GetHasGoodStoryConditions(Map map)
        {
            return map != null && CanFindPsychic(map, out _);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var map = parms.target as Map;
            bool result;
            if (GetHasGoodStoryConditions(map))
            {
                if (!CanFindPsychic(map, out var pawn))
                {
                    result = false;
                }
                else
                {
                    var randomInRange = TimeoutDaysRange.RandomInRange;

                    if (TileFinder.TryFindNewSiteTile(out var tile))
                    {
                        var site = (Site) WorldObjectMaker.MakeWorldObject(SiteDefOfReconAndDiscovery.RD_Adventure);
                        site.Tile = tile;
                        var faction = Faction.OfInsects;
                        site.SetFaction(faction);
                        site.AddPart(new SitePart(site, SiteDefOfReconAndDiscovery.RD_AbandonedCastle,
                            SiteDefOfReconAndDiscovery.RD_AbandonedCastle.Worker.GenerateDefaultParams(
                                StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction)));
                        var source = from net in map?.powerNetManager.AllNetsListForReading
                            where net.hasPowerSource
                            select net;
                        if (source.Any())
                        {
                            var osirisCasket = new SitePart(site, SiteDefOfReconAndDiscovery.RD_OsirisCasket,
                                SiteDefOfReconAndDiscovery.RD_OsirisCasket.Worker.GenerateDefaultParams(
                                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                            {
                                hidden = true
                            };
                            site.parts.Add(osirisCasket);
                        }

                        if (Rand.Value < 0.15f)
                        {
                            var weatherSat = new SitePart(site, SiteDefOfReconAndDiscovery.RD_WeatherSat,
                                SiteDefOfReconAndDiscovery.RD_WeatherSat.Worker.GenerateDefaultParams(
                                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                            {
                                hidden = true
                            };
                            site.parts.Add(weatherSat);
                        }

                        site.GetComponent<TimeoutComp>().StartTimeout(randomInRange * 60000);
                        if (Rand.Value < 0.25f)
                        {
                            var scatteredManhunters = new SitePart(site,
                                SiteDefOfReconAndDiscovery.RD_ScatteredManhunters,
                                SiteDefOfReconAndDiscovery.RD_ScatteredManhunters.Worker.GenerateDefaultParams(
                                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                            {
                                hidden = true
                            };
                            site.parts.Add(scatteredManhunters);
                        }

                        if (Rand.Value < 0.1f)
                        {
                            var scatteredTreasure = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredTreasure,
                                SiteDefOfReconAndDiscovery.RD_ScatteredTreasure.Worker.GenerateDefaultParams(
                                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                            {
                                hidden = true
                            };
                            site.parts.Add(scatteredTreasure);
                        }

                        if (Rand.Value < 1f)
                        {
                            var enemyRaidOnArrival = new SitePart(site,
                                SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival,
                                SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival.Worker.GenerateDefaultParams(
                                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                            {
                                hidden = true
                            };
                            site.parts.Add(enemyRaidOnArrival);
                        }

                        if (Rand.Value < 0.9f)
                        {
                            var enemyRaidOnArrival = new SitePart(site,
                                SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival,
                                SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival.Worker.GenerateDefaultParams(
                                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                            {
                                hidden = true
                            };
                            site.parts.Add(enemyRaidOnArrival);
                        }

                        if (Rand.Value < 0.6f)
                        {
                            var enemyRaidOnArrival = new SitePart(site,
                                SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival,
                                SiteDefOfReconAndDiscovery.RD_EnemyRaidOnArrival.Worker.GenerateDefaultParams(
                                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                            {
                                hidden = true
                            };
                            site.parts.Add(enemyRaidOnArrival);
                        }

                        Find.WorldObjects.Add(site);
                        var qi = new QueuedIncident(new FiringIncident(IncidentDef.Named("PsychicDrone"), null, parms),
                            Find.TickManager.TicksGame + 1);
                        Find.Storyteller.incidentQueue.Add(qi);
                        Find.LetterStack.ReceiveLetter("RD_PsychicMessage".Translate(),
                            "RD_ReceivedVisionBattle"
                                .Translate().Formatted(pawn.Named("PAWN"))
                                .AdjustedFor(pawn)
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
    }
}