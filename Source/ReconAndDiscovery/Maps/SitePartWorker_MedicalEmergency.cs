using System;
using System.Collections.Generic;
using ReconAndDiscovery.Missions;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ReconAndDiscovery.Maps
{
	public class SitePartWorker_MedicalEmergency : SitePartWorker
	{
		public override void PostMapGenerate(Map map)
		{
			base.PostMapGenerate(map);
            Faction faction = Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer);
            int maxPawns = Find.World.worldObjects.MapParentAt(map.Tile).GetComponent<QuestComp_MedicalEmergency>().maxPawns;
			List<Pawn> list = new List<Pawn>();
			IntVec3 baseCenter;
			if (RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((IntVec3 x) => x.Standable(map) && x.Fogged(map) && GridsUtility.GetRoom(x, map, RegionType.Set_Passable).CellCount >= 2, map, out baseCenter))
			{
				for (int i = 0; i < maxPawns; i++)
				{
					IntVec3 intVec;
					if (RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((IntVec3 x) => x.Standable(map) && x.Fogged(map) && GridsUtility.GetRoom(x, map, RegionType.Set_Passable).CellCount >= 2, map, out intVec))
					{
						PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDefOf.SpaceRefugee, faction, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, false, 20f, false, true, true, false, false);
						Pawn pawn = PawnGenerator.GeneratePawn(request);
						list.Add(pawn);
						HealthUtility.DamageUntilDowned(pawn);
						IntVec3 intVec2 = CellFinder.RandomSpawnCellForPawnNear(intVec, map, 18);
						GenSpawn.Spawn(pawn, intVec2, map, Rot4.Random, WipeMode.Vanish, false);
					}
				}
				LordJob_DefendBase lordJob = new LordJob_DefendBase(faction, baseCenter);
				LordMaker.MakeNewLord(faction, lordJob, map, list);
			}
		}

		public FloatRange casualtiesRange = new FloatRange(400f, 1000f);
	}
}

