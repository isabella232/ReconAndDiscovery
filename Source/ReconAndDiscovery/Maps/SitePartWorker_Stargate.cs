using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class SitePartWorker_Stargate : SitePartWorker
	{
		public override void PostMapGenerate(Map map)
		{
			base.PostMapGenerate(map);
			IEnumerable<Pawn> enumerable = from p in map.mapPawns.AllPawnsSpawned
            where p.Faction == Find.FactionManager.RandomNonHostileFaction(true, false, true, TechLevel.Spacer)
            select p;
			foreach (Pawn pawn in enumerable)
			{
				pawn.Destroy(DestroyMode.Vanish);
			}
			IntVec3 loc;
			if (RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((IntVec3 x) => x.Standable(map) && !x.Fogged(map), map, out loc))
			{
				Thing newThing = ThingMaker.MakeThing(ThingDef.Named("RD_Stargate"), null);
				GenSpawn.Spawn(newThing, loc, map);
				foreach (Pawn pawn2 in SitePartWorker_Stargate.tmpPawnsToSpawn)
				{
					if (pawn2.Spawned)
					{
						pawn2.DeSpawn();
					}
					GenSpawn.Spawn(pawn2, loc, map);
				}
				SitePartWorker_Stargate.tmpPawnsToSpawn.Clear();
			}
		}

		public static List<Pawn> tmpPawnsToSpawn = new List<Pawn>();
	}
}

