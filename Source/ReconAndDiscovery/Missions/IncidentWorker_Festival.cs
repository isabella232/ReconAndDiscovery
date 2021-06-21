using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions
{
    public class IncidentWorker_Festival : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out _);
        }

        public List<Faction> GetAllNonPlayerFriends(Faction faction)
        {
            var list = Find.FactionManager.AllFactionsVisible.ToList();
            if (list.Contains(Faction.OfPlayer))
            {
                list.Remove(Faction.OfPlayer);
            }

            return (from f in list
                where faction.GoodwillWith(f) > 10f
                select f).ToList();
        }

        public int FriendsCount(Faction faction)
        {
            var list = Find.FactionManager.AllFactionsVisible.ToList();
            if (list.Contains(faction))
            {
                list.Remove(faction);
            }

            list = (from f in list
                where faction.GoodwillWith(f) > 10f
                select f).ToList();
            return list.Count;
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
            if ((from wo in Find.WorldObjects.AllWorldObjects
                where wo.def == SiteDefOfReconAndDiscovery.RD_AdventurePeaceTalks
                select wo).Any())
            {
                return false;
            }

            if (!TryFindFaction(out var faction, f => f != Faction.OfPlayer && f.PlayerGoodwill >= 0))
            {
                return false;
            }

            if (!TileFinder.TryFindNewSiteTile(out var tile))
            {
                return false;
            }

            var site = (Site) WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Site);
            site.Tile = tile;
            site.SetFaction(faction);
            site.AddPart(new SitePart(site, SiteDefOfReconAndDiscovery.RD_Festival,
                SiteDefOfReconAndDiscovery.RD_Festival.Worker.GenerateDefaultParams(
                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction)));

            // TODO: check if this works correctly
            var outpost = new SitePart(site, SitePartDefOf.Outpost,
                SitePartDefOf.Outpost.Worker.GenerateDefaultParams(
                    StorytellerUtility.DefaultSiteThreatPointsNow(), tile, faction))
            {
                hidden = true
            };
            site.parts.Add(outpost);
            var num = 8;
            site.GetComponent<TimeoutComp>().StartTimeout(num * 60000);
            SendStandardLetter(parms, site, faction.Name);
            Find.WorldObjects.Add(site);
            return true;
        }
    }
}