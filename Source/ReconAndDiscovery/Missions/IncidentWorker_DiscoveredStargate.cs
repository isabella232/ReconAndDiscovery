using System;
using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace ReconAndDiscovery.Missions
{
	public class IncidentWorker_DiscoveredStargate : IncidentWorker
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			int num;
			return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out num);
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
            int tile = -1;
            bool result;
            Faction faction = Find.FactionManager.RandomEnemyFaction(false, false, true);
            for (int i = 0; i < 20; i++)
            {
                tile = TileFinder.RandomSettlementTileFor(faction, false, null);
                if (TileFinder.IsValidTileForNewSettlement(tile, null))
                {
                    break;
                }
                else
                {
                    tile = -1;
                }
            }
            if (tile != -1)
            {
                Site site = (Site)WorldObjectMaker.MakeWorldObject(SiteDefOfReconAndDiscovery.RD_Adventure);
                site.Tile = tile;
                site.SetFaction(faction);
                SitePart starGate = new SitePart(site, SiteDefOfReconAndDiscovery.RD_Stargate, SiteDefOfReconAndDiscovery.RD_Stargate.Worker.GenerateDefaultParams
(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
                site.AddPart(starGate);
                float value = Rand.Value;
                if ((double)value < 0.25) 
                {
                    SitePart abandonedCastle = new SitePart(site, SiteDefOfReconAndDiscovery.RD_AbandonedCastle,
    SiteDefOfReconAndDiscovery.RD_AbandonedCastle.Worker.GenerateDefaultParams
    (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
                    abandonedCastle.hidden = true;
                    site.AddPart(abandonedCastle);
                }
                else if ((double)value < 0.50)
                {
                    SitePart abandonedColony = new SitePart(site, SiteDefOfReconAndDiscovery.RD_AbandonedColony,
SiteDefOfReconAndDiscovery.RD_AbandonedColony.Worker.GenerateDefaultParams
(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
                    abandonedColony.hidden = true;
                    site.AddPart(abandonedColony);
                }
                else if ((double)value < 0.75)
                {
                    SitePart preciousLump = new SitePart(site, SitePartDefOf.PreciousLump,
SitePartDefOf.PreciousLump.Worker.GenerateDefaultParams
(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
                    preciousLump.hidden = true;
                    site.AddPart(preciousLump);
                }
                else
                {
                    site = (Site)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Site);
                    site.Tile = tile;
                    site.SetFaction(faction);
                    SitePart starGate2 = new SitePart(site, SiteDefOfReconAndDiscovery.RD_Stargate, SiteDefOfReconAndDiscovery.RD_Stargate.Worker.GenerateDefaultParams
(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
                    site.AddPart(starGate2);
                    // TODO: check if this works correctly
                    SitePart outpost = new SitePart(site, SitePartDefOf.Outpost, SitePartDefOf.Outpost.Worker.GenerateDefaultParams
(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
                    outpost.hidden = true;
                    site.parts.Add(outpost);
                    SitePart turrets = new SitePart(site, SitePartDefOf.Turrets, SitePartDefOf.Turrets.Worker.GenerateDefaultParams
(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
                    turrets.hidden = true;
                    site.parts.Add(turrets);
                }
                if (Rand.Value < 0.2f)
                {
                    SitePart scatteredManhunters = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredManhunters, SiteDefOfReconAndDiscovery.RD_ScatteredManhunters.Worker.GenerateDefaultParams
(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));

                    scatteredManhunters.hidden = true;

                    site.parts.Add(scatteredManhunters);
                }
                if (Rand.Value < 0.85f) 
                {
                    SitePart scatteredTreasure = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredTreasure,
                    SiteDefOfReconAndDiscovery.RD_ScatteredTreasure.Worker.GenerateDefaultParams
                    (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));

                    scatteredTreasure.hidden = true;

                    site.parts.Add(scatteredTreasure);
                }
                site.GetComponent<TimeoutComp>().StartTimeout(10 * 60000);
                base.SendStandardLetter(parms, site);
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

