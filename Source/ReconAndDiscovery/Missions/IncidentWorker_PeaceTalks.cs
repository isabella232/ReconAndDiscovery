using System;
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
            return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out _);
        }

        private bool TryFindFaction(out Faction faction, Predicate<Faction> validator)
        {
            faction = null;
            var list = Find.FactionManager.AllFactionsVisible.ToList();
            if (list.Contains(Faction.OfPlayer))
            {
                list.Remove(Faction.OfPlayer);
            }

            list = (from f in list
                where validator(f)
                select f).ToList();
            bool result;
            if (list.Any())
            {
                faction = list.RandomElement();
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
            if ((from wo in Find.WorldObjects.AllWorldObjects
                where wo.def == SiteDefOfReconAndDiscovery.RD_AdventurePeaceTalks
                select wo).Any())
            {
                result = false;
            }
            else if (!TryFindFaction(out var faction,
                f => f != Faction.OfPlayer && f.PlayerGoodwill < 0f && f.def.CanEverBeNonHostile &&
                     f.def.humanlikeFaction))
            {
                result = false;
            }
            else
            {
                if (TileFinder.TryFindNewSiteTile(out var tile))
                {
                    var site = (Site) WorldObjectMaker.MakeWorldObject(
                        SiteDefOfReconAndDiscovery.RD_AdventurePeaceTalks);
                    site.Tile = tile;
                    site.SetFaction(faction);
                    site.AddPart(new SitePart(site, SiteDefOfReconAndDiscovery.RD_PeaceTalks,
                        SiteDefOfReconAndDiscovery.RD_PeaceTalks.Worker.GenerateDefaultParams(
                            StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction)));

                    var peaceTalksFaction = new SitePart(site, SiteDefOfReconAndDiscovery.RD_PeaceTalksFaction,
                        SiteDefOfReconAndDiscovery.RD_PeaceTalksFaction.Worker.GenerateDefaultParams(
                            StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
                    {
                        hidden = true
                    };
                    site.parts.Add(peaceTalksFaction);
                    site.GetComponent<QuestComp_PeaceTalks>().StartQuest(faction);
                    var num = 5;
                    site.GetComponent<TimeoutComp>().StartTimeout(num * 60000);
                    SendStandardLetter(parms, site, faction.leader.Label, faction.Name, num.ToString());
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