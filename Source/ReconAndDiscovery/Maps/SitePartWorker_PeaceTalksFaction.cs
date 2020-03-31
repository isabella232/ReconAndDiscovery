using System;
using System.Collections.Generic;
using ReconAndDiscovery.Missions;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace ReconAndDiscovery.Maps
{
	public class SitePartWorker_PeaceTalksFaction : SitePartWorker
	{
		public override void PostMapGenerate(Map map)
		{
			base.PostMapGenerate(map);
			MapParent mapParent = Find.World.worldObjects.MapParentAt(map.Tile);
			Faction faction = mapParent.Faction;
            //TODO: check if it works
			IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.FactionArrival, map);
			incidentParms.points = Mathf.Max(incidentParms.points, 250f);
			incidentParms.points *= 2f;
			PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms();
			pawnGroupMakerParms.groupKind = PawnGroupKindDefOf.Settlement;
			pawnGroupMakerParms.tile = map.Tile;
			pawnGroupMakerParms.faction = faction;
			pawnGroupMakerParms.points = incidentParms.points;
			pawnGroupMakerParms.inhabitants = true;
			List<Pawn> list = new List<Pawn>();
			foreach (Pawn pawn in PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms, true))
			{
				IntVec3 loc;
				CellFinder.TryFindRandomCellInsideWith(new CellRect(40, 40, map.Size.x - 80, map.Size.z - 80), (IntVec3 c) => c.Standable(map), out loc);
				GenSpawn.Spawn(pawn, loc, map);
				list.Add(pawn);
			}
			IntVec3 intVec;
			CellFinder.TryFindRandomCellInsideWith(new CellRect(50, 50, map.Size.x - 100, map.Size.z - 100), (IntVec3 c) => c.Standable(map), out intVec);
			if (faction.leader != null)
			{
				GenSpawn.Spawn(faction.leader, intVec, map);
				mapParent.GetComponent<QuestComp_PeaceTalks>().Negotiator = faction.leader;
				list.Add(faction.leader);
			}
			LordJob lordJob = new LordJob_DefendBase(faction, intVec);
			LordMaker.MakeNewLord(faction, lordJob, map, list);
		}

		public FloatRange casualtiesRange = new FloatRange(400f, 1000f);
	}
}

