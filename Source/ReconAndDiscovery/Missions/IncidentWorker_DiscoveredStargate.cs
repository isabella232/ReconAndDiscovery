using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions
{
    public class IncidentWorker_DiscoveredStargate : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out _);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var tile = -1;
            bool result;
            var faction = Find.FactionManager.RandomEnemyFaction();
            for (var i = 0; i < 20; i++)
            {
                tile = TileFinder.RandomSettlementTileFor(faction);
                if (TileFinder.IsValidTileForNewSettlement(tile))
                {
                    break;
                }

                tile = -1;
            }

            if (tile != -1)
            {
                var site = (Site) WorldObjectMaker.MakeWorldObject(SiteDefOfReconAndDiscovery.RD_Adventure);
                site.Tile = tile;
                site.SetFaction(faction);
                var starGate = new SitePart(site, SiteDefOfReconAndDiscovery.RD_Stargate,
                    SiteDefOfReconAndDiscovery.RD_Stargate.Worker.GenerateDefaultParams
                        (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
                site.AddPart(starGate);
                var value = Rand.Value;
                if (value < 0.25)
                {
                    var abandonedCastle = new SitePart(site, SiteDefOfReconAndDiscovery.RD_AbandonedCastle,
                        SiteDefOfReconAndDiscovery.RD_AbandonedCastle.Worker.GenerateDefaultParams
                            (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                    {
                        hidden = true
                    };
                    site.AddPart(abandonedCastle);
                }
                else if (value < 0.50)
                {
                    var abandonedColony = new SitePart(site, SiteDefOfReconAndDiscovery.RD_AbandonedColony,
                        SiteDefOfReconAndDiscovery.RD_AbandonedColony.Worker.GenerateDefaultParams
                            (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                    {
                        hidden = true
                    };
                    site.AddPart(abandonedColony);
                }
                else if (value < 0.75)
                {
                    var preciousLump = new SitePart(site, SitePartDefOf.PreciousLump,
                        SitePartDefOf.PreciousLump.Worker.GenerateDefaultParams
                            (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                    {
                        hidden = true
                    };
                    site.AddPart(preciousLump);
                }
                else
                {
                    site = (Site) WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Site);
                    site.Tile = tile;
                    site.SetFaction(faction);
                    var starGate2 = new SitePart(site, SiteDefOfReconAndDiscovery.RD_Stargate,
                        SiteDefOfReconAndDiscovery.RD_Stargate.Worker.GenerateDefaultParams
                            (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
                    site.AddPart(starGate2);
                    // TODO: check if this works correctly
                    var outpost = new SitePart(site, SitePartDefOf.Outpost,
                        SitePartDefOf.Outpost.Worker.GenerateDefaultParams
                            (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                    {
                        hidden = true
                    };
                    site.parts.Add(outpost);
                    var turrets = new SitePart(site, SitePartDefOf.Turrets,
                        SitePartDefOf.Turrets.Worker.GenerateDefaultParams
                            (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                    {
                        hidden = true
                    };
                    site.parts.Add(turrets);
                }

                if (Rand.Value < 0.2f)
                {
                    var scatteredManhunters = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredManhunters,
                        SiteDefOfReconAndDiscovery.RD_ScatteredManhunters.Worker.GenerateDefaultParams
                            (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                    {
                        hidden = true
                    };

                    site.parts.Add(scatteredManhunters);
                }

                if (Rand.Value < 0.85f)
                {
                    var scatteredTreasure = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredTreasure,
                        SiteDefOfReconAndDiscovery.RD_ScatteredTreasure.Worker.GenerateDefaultParams
                            (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                    {
                        hidden = true
                    };

                    site.parts.Add(scatteredTreasure);
                }

                site.GetComponent<TimeoutComp>().StartTimeout(10 * 60000);
                SendStandardLetter(parms, site);
                Find.WorldObjects.Add(site);
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }
    }
}