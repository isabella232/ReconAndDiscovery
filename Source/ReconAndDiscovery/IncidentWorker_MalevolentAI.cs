using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery
{
    public class IncidentWorker_MalevolentAI : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var map = (Map) parms.target;
            bool result;
            if (map == null)
            {
                result = false;
            }
            else
            {
                var source = from p in map.mapPawns.AllPawnsSpawned
                    where p.Faction.HostileTo(Faction.OfPlayer)
                          && GenHostility.IsActiveThreatTo(p, Faction.OfPlayer)
                    select p;

                if (!source.Any())
                {
                    result = false;
                }
                else
                {
                    var pawn = source.RandomElement();
                    var list = (from b in map.listerBuildings.allBuildingsColonist
                        where b.def.building.ai_combatDangerous && b.GetComp<CompPowerTrader>() != null &&
                              b.GetComp<CompPowerTrader>().PowerOn
                        select b).ToList();
                    if (!list.Any())
                    {
                        result = false;
                    }
                    else
                    {
                        foreach (var building in list)
                        {
                            building.SetFaction(pawn.Faction);
                        }

                        SendStandardLetter(parms, list.FirstOrDefault(), pawn.Faction.Name);
                        result = true;
                    }
                }
            }

            return result;
        }
    }
}