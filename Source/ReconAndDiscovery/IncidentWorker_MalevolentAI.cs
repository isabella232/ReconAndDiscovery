using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery
{
	public class IncidentWorker_MalevolentAI : IncidentWorker
	{
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			bool result;
			if (map == null) 
			{
				result = false;
			}
			else
			{
                IEnumerable<Pawn> source = from p in map.mapPawns.AllPawnsSpawned
				where p.Faction.HostileTo(Faction.OfPlayer) 
                && GenHostility.IsActiveThreatTo(p, Faction.OfPlayer)
                select p;

				if (source.Count<Pawn>() == 0)
				{
					result = false;
				}
				else
				{
					Pawn pawn = source.RandomElement<Pawn>();
					List<Building> list = (from b in map.listerBuildings.allBuildingsColonist
					where b.def.building.ai_combatDangerous && b.GetComp<CompPowerTrader>() != null && b.GetComp<CompPowerTrader>().PowerOn
					select b).ToList<Building>();
					if (list.Count<Building>() == 0)
					{
						result = false;
					}
					else
					{
						foreach (Building building in list)
						{
							building.SetFaction(pawn.Faction, null);
						}
						base.SendStandardLetter(parms, list.FirstOrDefault<Building>(), new NamedArgument[]
						{
							pawn.Faction.Name
						});
						result = true;
					}
				}
			}
			return result;
		}
	}
}

