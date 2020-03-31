using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions
{
    public class IncidentWorker_PeaceTalks : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            int num;
            return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out num);
        }

        public bool TryFindFaction(out Faction faction, Predicate<Faction> validator)
        {
            faction = null;
            List<Faction> list = Find.FactionManager.AllFactionsVisible.ToList<Faction>();
            if (list.Contains(Faction.OfPlayer))
            {
                list.Remove(Faction.OfPlayer);
            }
            list = (from f in list
                    where validator(f)
                    select f).ToList<Faction>();
            bool result;
            if (list.Count<Faction>() > 0)
            {
                faction = list.RandomElement<Faction>();
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            bool result;
            Faction faction;
            if ((from wo in Find.WorldObjects.AllWorldObjects
                 where wo.def == SiteDefOfReconAndDiscovery.RD_AdventurePeaceTalks
                 select wo).Count<WorldObject>() > 0)
            {
                result = false;
            }
            else if (!this.TryFindFaction(out faction, (Faction f) => f != Faction.OfPlayer && f.PlayerGoodwill < 0f && f.def.CanEverBeNonHostile && f.def.humanlikeFaction))
            {
                result = false;
            }
            else
            {
                int tile;
                if (TileFinder.TryFindNewSiteTile(out tile))
                {
                    Site site = (Site)WorldObjectMaker.MakeWorldObject(SiteDefOfReconAndDiscovery.RD_AdventurePeaceTalks);
                    site.Tile = tile;
                    site.SetFaction(faction); 
                    site.AddPart(new SitePart(site, SiteDefOfReconAndDiscovery.RD_PeaceTalks,
SiteDefOfReconAndDiscovery.RD_PeaceTalks.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction)));

                    SitePart peaceTalksFaction = new SitePart(site, SiteDefOfReconAndDiscovery.RD_PeaceTalksFaction, SiteDefOfReconAndDiscovery.RD_PeaceTalksFaction.Worker.GenerateDefaultParams(StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction));
                    peaceTalksFaction.hidden = true;
                    site.parts.Add(peaceTalksFaction);
                    site.GetComponent<QuestComp_PeaceTalks>().StartQuest(faction);
                    int num = 5;
                    site.GetComponent<TimeoutComp>().StartTimeout(num * 60000);
                    base.SendStandardLetter(parms, site, new NamedArgument[]
                    {
                        faction.leader.Label,
                        faction.Name,
                        num.ToString()
                    });
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

