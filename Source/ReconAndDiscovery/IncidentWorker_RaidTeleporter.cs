using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ReconAndDiscovery
{
	public class IncidentWorker_RaidTeleporter : IncidentWorker_RaidEnemy
	{
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			IEnumerable<Pawn> source = from p in map.mapPawns.AllPawnsSpawned
			where p.Faction == Faction.OfMechanoids && !p.Downed
			select p;
			bool result;
			if (source.Count<Pawn>() == 0)
			{
				result = false;
			}
			else
			{
				IEnumerable<Building> enumerable = from b in map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("RD_Teleporter"))
				where b.GetComp<CompPowerTrader>().PowerOn && b.GetComp<CompTeleporter>().ReadyToTransport
				select b;
				if (enumerable.Count<Building>() == 0)
				{
					result = false;
				}
				else
				{
					List<Pawn> list = new List<Pawn>();
					Pawn p2 = source.RandomElement<Pawn>();
					foreach (Building building in enumerable)
					{
						Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDef.Named("Scyther"), Faction.OfMechanoids);
						GenSpawn.Spawn(pawn, building.Position, building.Map);
						building.GetComp<CompTeleporter>().ResetCharge();
						list.Add(pawn);
						p2.GetLord().AddPawn(pawn);
					}
					base.SendStandardLetter(parms, list.FirstOrDefault<Pawn>());
					Find.TickManager.slower.SignalForceNormalSpeedShort();
					Find.StoryWatcher.statsRecord.numRaidsEnemy++;
					result = true;
				}
			}
			return result;
		}
	}
}

