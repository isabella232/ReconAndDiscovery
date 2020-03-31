using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class SitePartWorker_HoloDisk : SitePartWorker
	{
		public override void PostMapGenerate(Map map)
		{
			base.PostMapGenerate(map);
			IEnumerable<Pawn> enumerable = from p in map.mapPawns.AllPawnsSpawned
			where p.Faction == Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer)
            select p;
			foreach (Pawn pawn in enumerable)
			{
				pawn.Destroy(DestroyMode.Vanish);
			}
			IntVec3 loc;
			if (RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((IntVec3 x) => x.Standable(map) && x.Fogged(map) && GridsUtility.GetRoom(x, map, RegionType.Set_Passable).CellCount <= 30, map, out loc))
			{
				Thing newThing = ThingMaker.MakeThing(ThingDef.Named("RD_HoloDisk"), null);
				GenSpawn.Spawn(newThing, loc, map);
			}
		}
	}
}

