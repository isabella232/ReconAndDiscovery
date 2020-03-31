using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions
{
    public class IncidentWorker_MuffaloHerd : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            int num;
            return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out num);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = parms.target as Map;
            int num = IncidentWorker_MuffaloHerd.TimeoutDaysRange.RandomInRange;
            int num2 = -1;
            string text;
            if (map != null)
            {
                if (!TileFinder.TryFindNewSiteTile(out num2))
                {
                    return false;
                }
                text = "RD_LargeMuffaloMigration".Translate()

//A large muffalo migration is due to pass near here in

+ " {0} " + "Days".Translate(); //days!
            }
            else
            {
                List<Caravan> list = (from c in Find.World.worldObjects.Caravans
                                      where c.Faction == Faction.OfPlayer
                                      select c).ToList<Caravan>();
                if (list.Count == 0)
                {
                    return false;
                }
                Caravan caravan = list.RandomElement<Caravan>();
                num -= 3;
                TileFinder.TryFindPassableTileWithTraversalDistance(caravan.Tile, 1, 2, out num2, (int t) => !Find.WorldObjects.AnyMapParentAt(t), false);
                if (num2 == 0 || num2 == -1)
                {
                    return false;
                }
                text = "RD_YourCaravanSpottedMuffaloMigration".Translate(); //"Your caravan has spotted a huge muffalo migration!";
            }
            bool result;
            if (num2 == 0 || num2 == -1)
            {
                result = false;
            }
            else
            {
                Site site = (Site)WorldObjectMaker.MakeWorldObject(SiteDefOfReconAndDiscovery.RD_Adventure);
                site.Tile = num2;

                site.AddPart(new SitePart(site, SiteDefOfReconAndDiscovery.RD_MuffaloMigration,
SiteDefOfReconAndDiscovery.RD_MuffaloMigration.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), num2, null)));
                if (Rand.Value < 0.5f)
                {
                    SitePart scatteredTreasure = new SitePart(site, SiteDefOfReconAndDiscovery.RD_ScatteredTreasure, SiteDefOfReconAndDiscovery.RD_ScatteredTreasure.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), num2, null));
                    scatteredTreasure.hidden = true;
                    site.parts.Add(scatteredTreasure);
                }
                Find.LetterStack.ReceiveLetter("RD_MuffaloMigration".Translate(), text, LetterDefOf.PositiveEvent, site, null);
                site.GetComponent<TimeoutComp>().StartTimeout(num * 60000);
                Find.WorldObjects.Add(site);
                result = true;
            }
            return result;
        }

        private static readonly IntRange TimeoutDaysRange = new IntRange(7, 12);
    }
}

