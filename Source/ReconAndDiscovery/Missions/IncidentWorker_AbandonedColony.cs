using System;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions
{
	public class IncidentWorker_AbandonedColony : IncidentWorker
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
            return base.CanFireNowSub(parms);
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
            bool result;
            if (!(parms.target is Caravan caravan))
			{
				result = false;
			}
			else
			{
                Site site = (Site)WorldObjectMaker.MakeWorldObject(SiteDefOfReconAndDiscovery.RD_Adventure);
                TileFinder.TryFindPassableTileWithTraversalDistance(caravan.Tile, 1, 2, out int tile, (int t) => !Find.WorldObjects.AnyMapParentAt(t), false);
                site.Tile = tile;
                Faction faction = Find.FactionManager.RandomEnemyFaction(true, false, true, TechLevel.Spacer);
                site.SetFaction(faction);

                site.AddPart(new SitePart(site, SiteDefOfReconAndDiscovery.RD_AbandonedColony, SiteDefOfReconAndDiscovery.RD_AbandonedColony.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction)));

                SitePart holoDisk = new SitePart(site, SiteDefOfReconAndDiscovery.RD_HoloDisk, SiteDefOfReconAndDiscovery.RD_HoloDisk.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                {
                    hidden = true
                };
                site.parts.Add(holoDisk);
                if (Rand.Value < 0.3f)
                {
                    SitePart scatteredManhunters = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredManhunters, SiteDefOfReconAndDiscovery.RD_ScatteredManhunters.Worker.GenerateDefaultParams
                    (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                    {
                        hidden = true
                    };
                    site.parts.Add(scatteredManhunters);
                }
                if (Rand.Value < 0.1f)
                {
                    SitePart mechanoidForces = new SitePart(site, SiteDefOfReconAndDiscovery.RD_MechanoidForces, SiteDefOfReconAndDiscovery.RD_MechanoidForces.Worker.GenerateDefaultParams
                    (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                    {
                        hidden = true
                    };
                    site.parts.Add(mechanoidForces);
                }
                if (Rand.Value < 0.05f)
                {
                    SitePart stargate = new SitePart(site, SiteDefOfReconAndDiscovery.RD_Stargate, SiteDefOfReconAndDiscovery.RD_Stargate.Worker.GenerateDefaultParams
                    (StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                    {
                        hidden = true
                    };

                    site.parts.Add(stargate);
                }
                Find.WorldObjects.Add(site);
                if (site == null)
				{
					result = false;
				}
				else
				{
					base.SendStandardLetter(parms, caravan);
					result = true;
				}
			}
			return result;
		}
	}
}

